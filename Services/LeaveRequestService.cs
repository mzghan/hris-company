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
    private readonly IFlexibleBenefitRepository _benefitRepository;
    private readonly IReferenceRepository _refs;
    private readonly ILogger<LeaveRequestService> _logger;

    public LeaveRequestService(
        ILeaveRequestRepository leaveRepository,
        IEmployeeRepository employeeRepository,
        IApprovalService approvalService,
        ITransactionRunner transaction,
        IFlexibleBenefitRepository benefitRepository,
        IReferenceRepository refs,
        ILogger<LeaveRequestService> logger)
    {
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _approvalService = approvalService;
        _transaction = transaction;
        _benefitRepository = benefitRepository;
        _refs = refs;
        _logger = logger;
    }

    public async Task<LeaveRequestResponseDto> CreateAsync(int employeeId, LeaveRequestCreateDto dto)
    {
        if (dto.EndDate < dto.StartDate)
            throw new BadRequestException("EndDate tidak boleh sebelum StartDate.");

        if (dto.EndDate.DayNumber - dto.StartDate.DayNumber > 365)
            throw new BadRequestException("Rentang cuti maksimal 1 tahun.");

        var leaveTypeId = dto.LeaveTypeId;
        if (leaveTypeId <= 0)
            leaveTypeId = (await _benefitRepository.GetLeaveTypesAsync()).FirstOrDefault(x => x.Name == "Annual")?.Id ?? 0;
        var leaveType = await _benefitRepository.GetLeaveTypeAsync(leaveTypeId)
            ?? throw new BadRequestException("Jenis cuti tidak ditemukan.");

        var days = await CountWorkingDaysAsync(dto.StartDate, dto.EndDate);
        if (days <= 0)
            throw new BadRequestException("Rentang tanggal tidak mencakup hari kerja.");

        _ = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException("Employee tidak ditemukan.");

        if (await _leaveRepository.HasOverlapAsync(employeeId, dto.StartDate, dto.EndDate))
            throw new BadRequestException("Sudah ada pengajuan cuti (Pending/Approved) yang bertabrakan dengan tanggal ini.");

        // Saldo dicek sejak submit agar pengajuan tidak bisa menumpuk melebihi hak cuti.
        foreach (var part in await SplitByYearAsync(dto.StartDate, dto.EndDate))
        {
            var balance = await EnsureBalanceAsync(employeeId, dto.LeaveTypeId, part.Year);
            var pending = await GetPendingDaysAsync(employeeId, leaveTypeId, part.Start, part.End);
            var available = balance.Entitlement + balance.CarriedOver - balance.Used - balance.Sold - pending;
            if (available < part.Days)
                throw new BadRequestException($"Saldo {leaveType.Name} tahun {part.Year} tidak cukup. Tersedia {Math.Max(available, 0)} hari.");
        }

        return await _transaction.RunAsync(async () =>
        {
            var leave = await _leaveRepository.AddAsync(new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Days = days,
                Reason = dto.Reason?.Trim(),
                Status = LeaveRequestStatus.Pending
            });

            var summary = $"{leaveType.Name}: {dto.StartDate:dd MMM yyyy} - {dto.EndDate:dd MMM yyyy} ({days} hari kerja)";
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

        await _approvalService.CancelAsync(approval.Id, actor);
        var updated = await _leaveRepository.GetByIdAsync(leaveRequestId);
        return await ToDtoAsync(updated!, actor);
    }

    public async Task<List<LeaveBalance>> GetBalancesAsync(int employeeId, int year) =>
        await _benefitRepository.GetBalancesAsync(employeeId, year);

    public Task<List<LeaveType>> GetLeaveTypesAsync() => _benefitRepository.GetLeaveTypesAsync();

    public async Task<int> CountWorkingDaysAsync(DateOnly start, DateOnly end)
    {
        var holidays = await _benefitRepository.GetHolidaysAsync(start, end);
        var holidaySet = holidays.Select(x => x.Date).ToHashSet();
        var count = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
            if (d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday && !holidaySet.Contains(d))
                count++;
        return count;
    }

    private async Task<LeaveBalance> EnsureBalanceAsync(int employeeId, int leaveTypeId, int year)
    {
        var balance = await _benefitRepository.GetBalanceAsync(employeeId, leaveTypeId, year);
        if (balance is not null) return balance;

        var type = await _benefitRepository.GetLeaveTypeAsync(leaveTypeId)
            ?? throw new BadRequestException("Jenis cuti tidak ditemukan.");

        // Nilai entitlement awal sengaja hanya seed development untuk Annual; HR dapat
        // mengubah saldo lewat halaman Flexible Benefit.
        var entitlement = string.Equals(type.Name, "Annual", StringComparison.OrdinalIgnoreCase) ? 12 : 0;
        return await _benefitRepository.AddBalanceAsync(new LeaveBalance
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            Year = year,
            Entitlement = entitlement,
            Used = 0,
            Sold = 0,
            CarriedOver = 0
        });
    }

    private async Task<int> GetPendingDaysAsync(int employeeId, int leaveTypeId, DateOnly start, DateOnly end)
    {
        var rows = await _leaveRepository.GetByEmployeeAsync(employeeId);
        var total = 0;
        foreach (var row in rows.Where(x => x.LeaveTypeId == leaveTypeId && x.Status == LeaveRequestStatus.Pending))
        {
            var s = row.StartDate > start ? row.StartDate : start;
            var e = row.EndDate < end ? row.EndDate : end;
            if (s <= e) total += await CountWorkingDaysAsync(s, e);
        }
        return total;
    }

    private async Task<List<(int Year, DateOnly Start, DateOnly End, int Days)>> SplitByYearAsync(DateOnly start, DateOnly end)
    {
        var result = new List<(int, DateOnly, DateOnly, int)>();
        for (var year = start.Year; year <= end.Year; year++)
        {
            var s = year == start.Year ? start : new DateOnly(year, 1, 1);
            var e = year == end.Year ? end : new DateOnly(year, 12, 31);
            result.Add((year, s, e, await CountWorkingDaysAsync(s, e)));
        }
        return result;
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
        LeaveTypeId = l.LeaveTypeId,
        LeaveTypeName = l.LeaveType?.Name ?? string.Empty,
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
