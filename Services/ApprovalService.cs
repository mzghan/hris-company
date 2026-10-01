using HRIS.Api.Common;
using HRIS.Api.DTOs.Approval;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class ApprovalService : IApprovalService
{
    private readonly IApprovalRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly ITransactionRunner _transaction;
    private readonly IEnumerable<IApprovalHandler> _handlers;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(
        IApprovalRepository repository,
        IEmployeeRepository employeeRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        IAuditLogService auditLogService,
        ITransactionRunner transaction,
        IEnumerable<IApprovalHandler> handlers,
        ILogger<ApprovalService> logger)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _auditLogService = auditLogService;
        _transaction = transaction;
        _handlers = handlers;
        _logger = logger;
    }

    // ================= Submit =================

    public async Task<ApprovalRequestResponseDto> SubmitAsync(
        string requestType, int requestRefId, int requesterEmployeeId, string summary, int? requestedDays = null)
    {
        return await _transaction.RunAsync(async () =>
        {
            if (await _repository.GetByRefAsync(requestType, requestRefId) is not null)
                throw new BadRequestException("Pengajuan ini sudah punya proses approval.");

            var requester = await _employeeRepository.GetByIdAsync(requesterEmployeeId)
                ?? throw new NotFoundException("Employee pengaju tidak ditemukan.");

            var flow = await _repository.GetActiveFlowStepsAsync(requestType);
            if (flow.Count == 0)
                throw new BadRequestException($"Alur approval untuk '{ApprovalRequestTypes.Label(requestType)}' belum dikonfigurasi.");

            // Rantai atasan diambil sekali, sedalam langkah ManagerChain yang paling dalam.
            var maxDepth = flow
                .Where(f => f.ApproverType == ApproverType.ManagerChain)
                .Select(f => f.ChainDepth ?? 1)
                .DefaultIfEmpty(0)
                .Max();
            var chain = maxDepth > 0
                ? await _employeeRepository.GetManagerChainAsync(requesterEmployeeId, maxDepth)
                : new List<Employee>();

            var steps = new List<ApprovalStep>();
            foreach (var f in flow)
            {
                // Langkah bersyarat (mis. cuti panjang): dilewati kalau jumlah hari belum cukup.
                if (f.MinRequestedDays is not null && (requestedDays ?? 0) < f.MinRequestedDays.Value)
                    continue;

                switch (f.ApproverType)
                {
                    case ApproverType.ManagerChain:
                        var depth = f.ChainDepth ?? 1;
                        // Rantai habis sebelum depth tercapai: langkah dilewati (bukan error).
                        if (chain.Count < depth) continue;
                        steps.Add(new ApprovalStep
                        {
                            Level = steps.Count + 1,
                            ApproverType = ApproverType.Employee,
                            ApproverEmployeeId = chain[depth - 1].Id
                        });
                        break;

                    case ApproverType.Role:
                        steps.Add(new ApprovalStep
                        {
                            Level = steps.Count + 1,
                            ApproverType = ApproverType.Role,
                            ApproverRoleId = f.RoleId
                                ?? throw new BadRequestException("Konfigurasi alur approval tidak lengkap: role belum diisi.")
                        });
                        break;

                    case ApproverType.Employee:
                        // Pengaju tidak menyetujui pengajuannya sendiri.
                        if (f.ApproverEmployeeId is null || f.ApproverEmployeeId == requesterEmployeeId) continue;
                        steps.Add(new ApprovalStep
                        {
                            Level = steps.Count + 1,
                            ApproverType = ApproverType.Employee,
                            ApproverEmployeeId = f.ApproverEmployeeId
                        });
                        break;
                }
            }

            if (steps.Count == 0)
                throw new BadRequestException(
                    "Tidak ada approver yang bisa ditentukan untuk pengajuan ini. Pastikan karyawan punya atasan langsung, atau hubungi HR.");

            var request = new ApprovalRequest
            {
                RequestType = requestType,
                RequestRefId = requestRefId,
                RequesterId = requesterEmployeeId,
                Summary = summary,
                Status = ApprovalRequestStatus.Pending,
                CurrentLevel = 1,
                Steps = steps
            };
            await _repository.AddAsync(request);

            _logger.LogInformation(
                "Approval {Id} ({Type} #{Ref}) dibuat dengan {Count} langkah",
                request.Id, requestType, requestRefId, steps.Count);

            await NotifyApproversAsync(request, steps[0], requester.FullName);

            var created = await _repository.GetByIdAsync(request.Id);
            return ToDto(created!, null);
        });
    }

    // PAF memakai approver dinamis: manager lama -> manager baru -> HR.
    // Manager baru tidak bisa disimpan sebagai REF_Approval_Flow_Step karena nilainya
    // bergantung pada isi pengajuan, sehingga tetap di-snapshot saat submit.
    public async Task<ApprovalRequestResponseDto> SubmitPersonalActionAsync(
        int requestRefId, int requesterEmployeeId, int? oldManagerId, int? newManagerId, string summary)
    {
        return await _transaction.RunAsync(async () =>
        {
            if (await _repository.GetByRefAsync(ApprovalRequestTypes.PersonalAction, requestRefId) is not null)
                throw new BadRequestException("PAF ini sudah punya proses approval.");

            var requester = await _employeeRepository.GetByIdAsync(requesterEmployeeId)
                ?? throw new NotFoundException("Employee pengaju tidak ditemukan.");

            var flow = await _repository.GetActiveFlowStepsAsync(ApprovalRequestTypes.PersonalAction);
            var hrFlow = flow.FirstOrDefault(f => f.ApproverType == ApproverType.Role && f.RoleId is not null);
            if (hrFlow is null)
                throw new BadRequestException("Alur Personal Action belum memiliki langkah HR.");

            var steps = new List<ApprovalStep>();
            void AddManager(int? managerId)
            {
                if (managerId is null || managerId == requesterEmployeeId) return;
                if (steps.Any(s => s.ApproverEmployeeId == managerId)) return;
                steps.Add(new ApprovalStep
                {
                    Level = steps.Count + 1,
                    ApproverType = ApproverType.Employee,
                    ApproverEmployeeId = managerId
                });
            }

            AddManager(oldManagerId);
            AddManager(newManagerId);
            steps.Add(new ApprovalStep
            {
                Level = steps.Count + 1,
                ApproverType = ApproverType.Role,
                ApproverRoleId = hrFlow.RoleId
            });

            var request = new ApprovalRequest
            {
                RequestType = ApprovalRequestTypes.PersonalAction,
                RequestRefId = requestRefId,
                RequesterId = requesterEmployeeId,
                Summary = summary,
                Status = ApprovalRequestStatus.Pending,
                CurrentLevel = 1,
                Steps = steps
            };
            await _repository.AddAsync(request);
            await NotifyApproversAsync(request, steps[0], requester.FullName);
            var created = await _repository.GetByIdAsync(request.Id);
            return ToDto(created!, null);
        });
    }

    // ================= Baca =================

    public async Task<ApprovalRequestResponseDto> GetByIdAsync(int approvalId, UserContext actor)
    {
        var request = await _repository.GetByIdAsync(approvalId)
            ?? throw new NotFoundException($"Approval dengan id {approvalId} tidak ditemukan.");

        var canView = actor.IsHrOrSupport
                      || (actor.EmployeeId is not null && actor.EmployeeId == request.RequesterId)
                      || request.Steps.Any(s => IsAssigned(s, actor));
        if (!canView)
            throw new ForbiddenException("Kamu tidak punya akses ke pengajuan ini.");

        return ToDto(request, actor);
    }

    public async Task<Dictionary<int, ApprovalRequestResponseDto>> GetByRefsAsync(
        string requestType, IEnumerable<int> requestRefIds, UserContext? actor = null)
    {
        var requests = await _repository.GetByRefsAsync(requestType, requestRefIds);
        return requests.ToDictionary(r => r.RequestRefId, r => ToDto(r, actor));
    }

    public async Task<List<ApprovalRequestResponseDto>> GetInboxAsync(UserContext actor, bool all = false)
    {
        if (all)
        {
            if (!actor.IsHrOrSupport)
                throw new ForbiddenException("Hanya HR/Support yang boleh melihat seluruh pengajuan yang berjalan.");

            var pending = await _repository.GetAllPendingAsync();
            return pending.Select(r => ToDto(r, actor)).ToList();
        }

        var inbox = await _repository.GetInboxAsync(actor.EmployeeId, actor.Roles);
        return inbox.Select(r => ToDto(r, actor)).ToList();
    }

    public async Task<List<ApprovalRequestResponseDto>> GetMyRequestsAsync(UserContext actor)
    {
        if (actor.EmployeeId is null)
            return new List<ApprovalRequestResponseDto>();

        var mine = await _repository.GetByRequesterAsync(actor.EmployeeId.Value);
        return mine.Select(r => ToDto(r, actor)).ToList();
    }

    // ================= Aksi =================

    public Task<ApprovalRequestResponseDto> ApproveAsync(int approvalId, UserContext actor, string? note) =>
        ActAsync(approvalId, actor, approve: true, note);

    public Task<ApprovalRequestResponseDto> RejectAsync(int approvalId, UserContext actor, string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new BadRequestException("Alasan penolakan wajib diisi.");

        return ActAsync(approvalId, actor, approve: false, note);
    }

    private async Task<ApprovalRequestResponseDto> ActAsync(int approvalId, UserContext actor, bool approve, string? note)
    {
        return await _transaction.RunAsync(async () =>
        {
            var request = await _repository.GetByIdAsync(approvalId)
                ?? throw new NotFoundException($"Approval dengan id {approvalId} tidak ditemukan.");

            if (request.Status != ApprovalRequestStatus.Pending)
                throw new BadRequestException("Pengajuan ini sudah selesai diproses.");

            // Tidak ada yang boleh memutuskan pengajuannya sendiri, termasuk Support.
            if (actor.EmployeeId is not null && actor.EmployeeId == request.RequesterId)
                throw new ForbiddenException("Kamu tidak bisa memutuskan pengajuanmu sendiri.");

            var step = request.Steps.FirstOrDefault(s => s.Level == request.CurrentLevel && s.Status == ApprovalStepStatus.Pending)
                ?? throw new BadRequestException("Tidak ada langkah approval yang sedang aktif.");

            var assigned = IsAssigned(step, actor);
            if (!assigned && !actor.IsSupport)
                throw new ForbiddenException("Kamu bukan approver untuk langkah yang sedang aktif pada pengajuan ini.");

            step.Status = approve ? ApprovalStepStatus.Approved : ApprovalStepStatus.Rejected;
            step.ActedByUserId = actor.UserId;
            step.ActedAt = DateTime.UtcNow;
            step.Note = note;
            step.IsSupportOverride = !assigned;

            ApprovalRequestStatus? finalStatus = null;
            if (approve)
            {
                var next = request.Steps.FirstOrDefault(s => s.Level == request.CurrentLevel + 1);
                if (next is not null)
                    request.CurrentLevel += 1;
                else
                    finalStatus = ApprovalRequestStatus.Approved;
            }
            else
            {
                finalStatus = ApprovalRequestStatus.Rejected;
            }

            if (finalStatus is not null)
                Complete(request, finalStatus.Value);

            await _repository.UpdateAsync(request);

            if (!assigned)
            {
                await _auditLogService.LogAsync(
                    actor.UserId,
                    approve ? "Approval.SupportApprove" : "Approval.SupportReject",
                    nameof(ApprovalRequest),
                    request.Id,
                    $"Support bertindak atas langkah {step.Level} ({ApprovalRequestTypes.Label(request.RequestType)} #{request.RequestRefId}).");
            }

            var requesterName = request.Requester?.FullName ?? "Karyawan";
            if (finalStatus is not null)
            {
                await RunHandlerAsync(request, finalStatus.Value);
                await NotifyRequesterAsync(request, finalStatus.Value, note);
            }
            else
            {
                var nextStep = request.Steps.First(s => s.Level == request.CurrentLevel);
                await NotifyApproversAsync(request, nextStep, requesterName);
            }

            _logger.LogInformation(
                "Approval {Id} langkah {Level} {Action} oleh user {UserId}",
                request.Id, step.Level, approve ? "disetujui" : "ditolak", actor.UserId);

            var updated = await _repository.GetByIdAsync(approvalId);
            return ToDto(updated!, actor);
        });
    }

    public async Task<ApprovalRequestResponseDto> CancelAsync(int approvalId, UserContext actor)
    {
        return await _transaction.RunAsync(async () =>
        {
            var request = await _repository.GetByIdAsync(approvalId)
                ?? throw new NotFoundException($"Approval dengan id {approvalId} tidak ditemukan.");

            var isRequester = actor.EmployeeId is not null && actor.EmployeeId == request.RequesterId;
            if (!isRequester && !actor.IsSupport)
                throw new ForbiddenException("Hanya pengaju yang boleh membatalkan pengajuan ini.");

            if (request.Status != ApprovalRequestStatus.Pending)
                throw new BadRequestException("Hanya pengajuan yang masih Pending yang bisa dibatalkan.");

            Complete(request, ApprovalRequestStatus.Cancelled);
            await _repository.UpdateAsync(request);

            if (!isRequester)
            {
                await _auditLogService.LogAsync(
                    actor.UserId, "Approval.SupportCancel", nameof(ApprovalRequest), request.Id,
                    $"Support membatalkan {ApprovalRequestTypes.Label(request.RequestType)} #{request.RequestRefId}.");
            }

            await RunHandlerAsync(request, ApprovalRequestStatus.Cancelled);

            var updated = await _repository.GetByIdAsync(approvalId);
            return ToDto(updated!, actor);
        });
    }

    // ================= Konfigurasi alur =================

    public async Task<List<ApprovalFlowStepResponseDto>> GetFlowStepsAsync()
    {
        var steps = await _repository.GetAllFlowStepsAsync();
        return steps.Select(ToFlowDto).ToList();
    }

    public async Task<ApprovalFlowStepResponseDto> UpdateFlowStepAsync(int id, ApprovalFlowStepUpdateDto dto)
    {
        var step = await _repository.GetFlowStepByIdAsync(id)
            ?? throw new NotFoundException($"Langkah alur approval dengan id {id} tidak ditemukan.");

        if (step.ApproverType == ApproverType.ManagerChain)
        {
            if (dto.ChainDepth is null || dto.ChainDepth < 1)
                throw new BadRequestException("ChainDepth minimal 1 untuk langkah tipe ManagerChain.");
            step.ChainDepth = dto.ChainDepth;
        }

        if (dto.MinRequestedDays is not null && dto.MinRequestedDays < 1)
            throw new BadRequestException("MinRequestedDays minimal 1 (atau kosongkan).");

        step.MinRequestedDays = dto.MinRequestedDays;
        step.IsActive = dto.IsActive;

        await _repository.UpdateFlowStepAsync(step);
        return ToFlowDto(step);
    }

    // ================= Helper =================

    private static void Complete(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        request.Status = finalStatus;
        request.CompletedAt = DateTime.UtcNow;

        // Langkah yang belum sempat berjalan ditandai Skipped.
        foreach (var s in request.Steps.Where(s => s.Status == ApprovalStepStatus.Pending))
            s.Status = ApprovalStepStatus.Skipped;
    }

    private async Task RunHandlerAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var handler = _handlers.FirstOrDefault(h => h.RequestType == request.RequestType);
        if (handler is null)
        {
            _logger.LogWarning("Belum ada IApprovalHandler untuk request_type '{Type}'", request.RequestType);
            return;
        }

        await handler.OnCompletedAsync(request, finalStatus);
    }

    private static bool IsAssigned(ApprovalStep step, UserContext actor) => step.ApproverType switch
    {
        ApproverType.Employee => actor.EmployeeId is not null && actor.EmployeeId == step.ApproverEmployeeId,
        ApproverType.Role => step.ApproverRole is not null && actor.Roles.Contains(step.ApproverRole.RoleName),
        _ => false
    };

    private async Task NotifyApproversAsync(ApprovalRequest request, ApprovalStep step, string requesterName)
    {
        var userIds = new List<int>();

        if (step.ApproverType == ApproverType.Employee && step.ApproverEmployeeId is int employeeId)
        {
            var userId = await _userRepository.GetUserIdByEmployeeIdAsync(employeeId);
            if (userId is not null) userIds.Add(userId.Value);
        }
        else if (step.ApproverType == ApproverType.Role && step.ApproverRoleId is int roleId)
        {
            userIds = await _userRepository.GetUserIdsByRoleIdAsync(roleId);
        }

        // Pengaju yang kebetulan pemegang role approver tidak perlu diberi tahu soal pengajuannya sendiri.
        var requesterUserId = await _userRepository.GetUserIdByEmployeeIdAsync(request.RequesterId);
        if (requesterUserId is not null)
            userIds.RemoveAll(id => id == requesterUserId.Value);

        await _notificationService.NotifyUsersAsync(
            userIds,
            $"Persetujuan {ApprovalRequestTypes.Label(request.RequestType)} menunggu",
            $"{requesterName}: {request.Summary}",
            "/Approvals/Index");
    }

    private async Task NotifyRequesterAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus, string? note)
    {
        // Dibatalkan oleh pengaju sendiri: tidak perlu notifikasi.
        if (finalStatus == ApprovalRequestStatus.Cancelled) return;

        var userId = await _userRepository.GetUserIdByEmployeeIdAsync(request.RequesterId);
        if (userId is null) return;

        var label = ApprovalRequestTypes.Label(request.RequestType);
        var verb = finalStatus == ApprovalRequestStatus.Approved ? "disetujui" : "ditolak";
        var message = $"{request.Summary} telah {verb}.";
        if (finalStatus == ApprovalRequestStatus.Rejected && !string.IsNullOrWhiteSpace(note))
            message += $" Alasan: {note}";

        await _notificationService.NotifyUsersAsync(
            new[] { userId.Value },
            $"Pengajuan {label} {verb}",
            message,
            ApprovalRequestTypes.RequesterLink(request.RequestType));
    }

    private static ApprovalRequestResponseDto ToDto(ApprovalRequest r, UserContext? actor)
    {
        var activeStep = r.Steps.FirstOrDefault(s => s.Level == r.CurrentLevel && s.Status == ApprovalStepStatus.Pending);

        var canAct = false;
        if (actor is not null && r.Status == ApprovalRequestStatus.Pending && activeStep is not null)
        {
            var isRequester = actor.EmployeeId is not null && actor.EmployeeId == r.RequesterId;
            canAct = !isRequester && (actor.IsSupport || IsAssigned(activeStep, actor));
        }

        return new ApprovalRequestResponseDto
        {
            Id = r.Id,
            RequestType = r.RequestType,
            RequestTypeLabel = ApprovalRequestTypes.Label(r.RequestType),
            RequestRefId = r.RequestRefId,
            RequesterId = r.RequesterId,
            RequesterName = r.Requester?.FullName ?? string.Empty,
            Summary = r.Summary,
            Status = r.Status.ToString(),
            CurrentLevel = r.CurrentLevel,
            CreatedAt = r.CreatedAt,
            CompletedAt = r.CompletedAt,
            CanAct = canAct,
            Steps = r.Steps
                .OrderBy(s => s.Level)
                .Select(s => new ApprovalStepResponseDto
                {
                    Id = s.Id,
                    Level = s.Level,
                    Status = s.Status.ToString(),
                    ApproverType = s.ApproverType.ToString(),
                    ApproverEmployeeId = s.ApproverEmployeeId,
                    ApproverName = s.ApproverType == ApproverType.Role
                        ? $"Role: {s.ApproverRole?.RoleName}"
                        : s.ApproverEmployee?.FullName ?? string.Empty,
                    ActedByUsername = s.ActedByUser?.Username,
                    IsSupportOverride = s.IsSupportOverride,
                    Note = s.Note,
                    ActedAt = s.ActedAt
                }).ToList()
        };
    }

    private static ApprovalFlowStepResponseDto ToFlowDto(ApprovalFlowStep f) => new()
    {
        Id = f.Id,
        RequestType = f.RequestType,
        RequestTypeLabel = ApprovalRequestTypes.Label(f.RequestType),
        Level = f.Level,
        ApproverType = f.ApproverType.ToString(),
        ChainDepth = f.ChainDepth,
        RoleName = f.Role?.RoleName,
        ApproverEmployeeName = f.ApproverEmployee?.FullName,
        MinRequestedDays = f.MinRequestedDays,
        IsActive = f.IsActive
    };
}
