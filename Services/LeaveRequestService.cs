using HRIS.Api.DTOs.Leave;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<LeaveRequestService> _logger;
    private readonly int _maxApprovalLevels;

    public LeaveRequestService(
        ILeaveRequestRepository leaveRepository,
        IEmployeeRepository employeeRepository,
        ILogger<LeaveRequestService> logger,
        IConfiguration config)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
        _maxApprovalLevels = int.Parse(config["LeaveApproval:MaxLevels"] ?? "2");
    }

    public async Task<LeaveRequestResponseDto> CreateAsync(int employeeId, LeaveRequestCreateDto dto)
    {
        if (dto.EndDate < dto.StartDate)
            throw new BadRequestException("EndDate tidak boleh sebelum StartDate.");

        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException("Employee tidak ditemukan.");

        // Susun rantai approver dengan menelusuri Employee.ManagerId ke atas,
        // maksimal sebanyak _maxApprovalLevels. Kalau employee tidak punya
        // manager sama sekali, leave request langsung tidak punya approval
        // (kasus tepi yang perlu didiskusikan lagi kalau muncul di data nyata).
        var managerChain = await _employeeRepository.GetManagerChainAsync(employeeId, _maxApprovalLevels);

        if (managerChain.Count == 0)
            throw new BadRequestException("Employee ini tidak punya manager terdaftar, tidak bisa membuat leave request.");

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveRequestStatus.Pending,
            CurrentLevel = 1
        };

        for (int i = 0; i < managerChain.Count; i++)
        {
            leaveRequest.Approvals.Add(new LeaveApproval
            {
                ApproverId = managerChain[i].Id,
                Level = i + 1,
                Status = ApprovalStatus.Pending
            });
        }

        await _leaveRepository.AddAsync(leaveRequest);
        _logger.LogInformation(
            "LeaveRequest {Id} dibuat untuk employee {EmployeeId} dengan {Levels} level approval",
            leaveRequest.Id, employeeId, managerChain.Count);

        var created = await _leaveRepository.GetByIdAsync(leaveRequest.Id);
        return ToDto(created!);
    }

    public async Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId)
    {
        var requests = await _leaveRepository.GetByEmployeeAsync(employeeId);
        return requests.Select(ToDto).ToList();
    }

    public async Task<List<LeaveRequestResponseDto>> GetPendingForApproverAsync(int approverEmployeeId)
    {
        var requests = await _leaveRepository.GetPendingForApproverAsync(approverEmployeeId);
        return requests.Select(ToDto).ToList();
    }

    public async Task<LeaveRequestResponseDto> ApproveAsync(int leaveRequestId, int approverEmployeeId, LeaveApprovalActionDto dto)
    {
        var (leaveRequest, currentApproval) = await GetActiveApprovalOrThrow(leaveRequestId, approverEmployeeId);

        currentApproval.Status = ApprovalStatus.Approved;
        currentApproval.Note = dto.Note;
        currentApproval.ActedAt = DateTime.UtcNow;

        var nextLevel = leaveRequest.Approvals.FirstOrDefault(a => a.Level == leaveRequest.CurrentLevel + 1);
        if (nextLevel is not null)
        {
            leaveRequest.CurrentLevel += 1;
        }
        else
        {
            leaveRequest.Status = LeaveRequestStatus.Approved;
        }

        await _leaveRepository.UpdateAsync(leaveRequest);
        _logger.LogInformation(
            "LeaveRequest {Id} di-approve oleh employee {ApproverId} di level {Level}",
            leaveRequestId, approverEmployeeId, currentApproval.Level);

        var updated = await _leaveRepository.GetByIdAsync(leaveRequestId);
        return ToDto(updated!);
    }

    public async Task<LeaveRequestResponseDto> RejectAsync(int leaveRequestId, int approverEmployeeId, LeaveApprovalActionDto dto)
    {
        var (leaveRequest, currentApproval) = await GetActiveApprovalOrThrow(leaveRequestId, approverEmployeeId);

        currentApproval.Status = ApprovalStatus.Rejected;
        currentApproval.Note = dto.Note;
        currentApproval.ActedAt = DateTime.UtcNow;
        leaveRequest.Status = LeaveRequestStatus.Rejected;

        await _leaveRepository.UpdateAsync(leaveRequest);
        _logger.LogInformation(
            "LeaveRequest {Id} di-reject oleh employee {ApproverId} di level {Level}",
            leaveRequestId, approverEmployeeId, currentApproval.Level);

        var updated = await _leaveRepository.GetByIdAsync(leaveRequestId);
        return ToDto(updated!);
    }

    private async Task<(LeaveRequest leaveRequest, LeaveApproval approval)> GetActiveApprovalOrThrow(int leaveRequestId, int approverEmployeeId)
    {
        var leaveRequest = await _leaveRepository.GetByIdAsync(leaveRequestId)
            ?? throw new NotFoundException($"LeaveRequest dengan id {leaveRequestId} tidak ditemukan.");

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new BadRequestException("LeaveRequest ini sudah tidak berstatus Pending.");

        var approval = leaveRequest.Approvals.FirstOrDefault(a => a.Level == leaveRequest.CurrentLevel)
            ?? throw new BadRequestException("Tidak ada approval aktif pada level ini.");

        if (approval.ApproverId != approverEmployeeId)
            throw new ForbiddenException("Anda bukan approver untuk level yang sedang aktif pada leave request ini.");

        return (leaveRequest, approval);
    }

    private static LeaveRequestResponseDto ToDto(LeaveRequest l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = l.Employee?.FullName ?? string.Empty,
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        Reason = l.Reason,
        Status = l.Status.ToString(),
        CurrentLevel = l.CurrentLevel,
        CreatedAt = l.CreatedAt,
        Approvals = l.Approvals
            .OrderBy(a => a.Level)
            .Select(a => new LeaveApprovalResponseDto
            {
                Id = a.Id,
                Level = a.Level,
                Status = a.Status.ToString(),
                ApproverId = a.ApproverId,
                ApproverName = a.Approver?.FullName ?? string.Empty,
                Note = a.Note,
                ActedAt = a.ActedAt
            }).ToList()
    };
}
