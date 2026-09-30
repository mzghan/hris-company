using HRIS.Api.Common;
using HRIS.Api.DTOs.Assistance;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class AssistanceRequestService : IAssistanceRequestService
{
    private readonly IAssistanceRequestRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly ITransactionRunner _transaction;

    public AssistanceRequestService(IAssistanceRequestRepository repository, IUserRepository userRepository,
        INotificationService notificationService, IAuditLogService auditLogService, ITransactionRunner transaction)
    {
        _repository = repository; _userRepository = userRepository; _notificationService = notificationService;
        _auditLogService = auditLogService; _transaction = transaction;
    }

    public async Task<List<AssistanceRequestResponseDto>> GetListAsync(UserContext actor)
    {
        if (actor.IsHrOrSupport)
            return (await _repository.GetAllAsync(includeAnonymous: true)).Select(ToDto).ToList();
        if (actor.EmployeeId is null) return new();
        return (await _repository.GetByEmployeeAsync(actor.EmployeeId.Value)).Select(ToDto).ToList();
    }

    public async Task<AssistanceRequestResponseDto> CreateAsync(AssistanceRequestCreateDto dto, UserContext actor)
    {
        if (!actor.IsHrOrSupport && actor.EmployeeId is null)
            throw new ForbiddenException("Akun ini tidak terhubung ke data Employee.");

        return await _transaction.RunAsync(async () =>
        {
            var anonymous = dto.IsAnonymous;
            var request = await _repository.AddAsync(new AssistanceRequest
            {
                EmployeeId = anonymous ? null : actor.EmployeeId,
                Category = dto.Category.Trim(),
                Description = dto.Description.Trim(),
                IsAnonymous = anonymous,
                Status = AssistanceRequestStatus.Pending
            });

            var hrRoles = await _userRepository.GetRolesByNamesAsync(new[] { RoleNames.HR });
            if (hrRoles.Count > 0)
            {
                var hrIds = await _userRepository.GetUserIdsByRoleIdAsync(hrRoles[0].Id);
                await _notificationService.NotifyUsersAsync(hrIds, "Permintaan EAP baru", anonymous
                    ? $"Ada permintaan EAP anonim baru pada kategori {request.Category}."
                    : $"Ada permintaan EAP baru dari karyawan pada kategori {request.Category}.", "/EAP/Index");
            }

            return ToDto((await _repository.GetByIdAsync(request.Id))!);
        });
    }

    public async Task<AssistanceRequestResponseDto> UpdateStatusAsync(int id, string status, UserContext actor)
    {
        if (!actor.IsHrOrSupport) throw new ForbiddenException("Hanya HR/Support yang boleh menangani permintaan EAP.");
        if (!AssistanceRequestStatus.IsValid(status)) throw new BadRequestException("Status EAP tidak valid.");
        var request = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Permintaan EAP tidak ditemukan.");
        request.Status = status;
        request.HandledByUserId = actor.UserId;
        await _repository.UpdateAsync(request);
        if (actor.IsSupport)
            await _auditLogService.LogAsync(actor.UserId, "AssistanceRequest.Status", nameof(AssistanceRequest), id, $"Status={status}; anonymous={request.IsAnonymous}");
        return ToDto((await _repository.GetByIdAsync(id))!);
    }

    private static AssistanceRequestResponseDto ToDto(AssistanceRequest x) => new()
    {
        Id = x.Id,
        EmployeeId = x.IsAnonymous ? null : x.EmployeeId,
        EmployeeName = x.IsAnonymous ? null : x.Employee?.FullName,
        Category = x.Category,
        Description = x.Description,
        IsAnonymous = x.IsAnonymous,
        Status = x.Status,
        HandledByName = x.HandledByUser?.Employee?.FullName ?? x.HandledByUser?.Username,
        CreatedAt = x.CreatedAt
    };
}
