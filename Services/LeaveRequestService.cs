using HRIS.Api.Common;
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
    private readonly IApprovalService _approvalService;
    private readonly ITransactionRunner _transaction;
    private readonly ILogger<LeaveRequestService> _logger;

    public LeaveRequestService(
        ILeaveRequestRepository leaveRepository,
        IEmployeeRepository employeeRepository,
        IApprovalService approvalService,
        ITransactionRunner transaction,
        ILogger<LeaveRequestService> logger)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _approvalService = approvalService;
        _transaction = transaction;
        _logger = logger;
    }

    public async Task<LeaveRequestResponseDto> CreateAsync(int employeeId, LeaveRequestCreateDto dto)
    {
        if (dto.EndDate < dto.StartDate)
            throw new BadRequestException("EndDate tidak boleh sebelum StartDate.");

        if (dto.EndDate.DayNumber - dto.StartDate.DayNumber > 365)
            throw new BadRequestException("Rentang cuti maksimal 1 tahun.");

        var days = CountWorkingDays(dto.StartDate, dto.EndDate);
        if (days <= 0)
            throw new BadRequestException("Rentang tanggal tidak mencakup hari kerja (Senin-Jumat).");

        _ = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException("Employee tidak ditemukan.");

        if (await _leaveRepository.HasOverlapAsync(employeeId, dto.StartDate, dto.EndDate))
            throw new BadRequestException("Sudah ada pengajuan cuti (Pending/Approved) yang bertabrakan dengan tanggal ini.");

        // Pengajuan dan approval-nya dibuat dalam satu transaction: kalau approver tidak bisa
        // ditentukan (mis. belum punya atasan), pengajuan cuti ikut dibatalkan.
        return await _transaction.RunAsync(async () =>
        {
            var leave = await _leaveRepository.AddAsync(new LeaveRequest
            {
                EmployeeId = employeeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Days = days,
                Reason = dto.Reason,
                Status = LeaveRequestStatus.Pending
            });

            var summary = $"Cuti {dto.StartDate:dd MMM yyyy} - {dto.EndDate:dd MMM yyyy} ({days} hari kerja)";
            await _approvalService.SubmitAsync(ApprovalRequestTypes.Leave, leave.Id, employeeId, summary, days);

            _logger.LogInformation("LeaveRequest {Id} dibuat untuk employee {EmployeeId} ({Days} hari kerja)", leave.Id, employeeId, days);

            var created = await _leaveRepository.GetByIdAsync(leave.Id);
            return await ToDtoAsync(created!, null);
        });
    }

    public async Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId)
    {
        var requests = await _leaveRepository.GetByEmployeeAsync(employeeId);
        var approvals = await _approvalService.GetByRefsAsync(ApprovalRequestTypes.Leave, requests.Select(r => r.Id));
        return requests.Select(r => ToDto(r, approvals.GetValueOrDefault(r.Id))).ToList();
    }

    public async Task<LeaveRequestResponseDto> CancelAsync(int leaveRequestId, UserContext actor)
    {
        var leave = await _leaveRepository.GetByIdAsync(leaveRequestId)
            ?? throw new NotFoundException($"LeaveRequest dengan id {leaveRequestId} tidak ditemukan.");

        var isOwner = actor.EmployeeId is not null && actor.EmployeeId == leave.EmployeeId;
        if (!isOwner && !actor.IsSupport)
            throw new ForbiddenException("Hanya pengaju yang boleh membatalkan cuti ini.");

        var approvals = await _approvalService.GetByRefsAsync(ApprovalRequestTypes.Leave, new[] { leaveRequestId });
        var approval = approvals.GetValueOrDefault(leaveRequestId)
            ?? throw new BadRequestException("Pengajuan cuti ini tidak punya proses approval.");

        // Status LeaveRequest ikut berubah lewat LeaveApprovalHandler di transaction yang sama.
        await _approvalService.CancelAsync(approval.Id, actor);

        var updated = await _leaveRepository.GetByIdAsync(leaveRequestId);
        return await ToDtoAsync(updated!, actor);
    }

    // Hari kerja = Senin-Jumat. Libur nasional baru bisa dikurangi setelah REF_Public_Holiday ada (Batch D).
    private static int CountWorkingDays(DateOnly start, DateOnly end)
    {
        var count = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                count++;
        }
        return count;
    }

    private async Task<LeaveRequestResponseDto> ToDtoAsync(LeaveRequest l, UserContext? actor)
    {
        var approvals = await _approvalService.GetByRefsAsync(ApprovalRequestTypes.Leave, new[] { l.Id }, actor);
        return ToDto(l, approvals.GetValueOrDefault(l.Id));
    }

    private static LeaveRequestResponseDto ToDto(LeaveRequest l, DTOs.Approval.ApprovalRequestResponseDto? approval) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = l.Employee?.FullName ?? string.Empty,
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        Days = l.Days,
        Reason = l.Reason,
        Status = l.Status.ToString(),
        CreatedAt = l.CreatedAt,
        ApprovalId = approval?.Id,
        CurrentLevel = approval?.CurrentLevel ?? 0,
        Approvals = approval?.Steps ?? new()
    };
}
