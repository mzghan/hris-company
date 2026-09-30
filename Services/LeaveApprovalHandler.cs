using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

// Mencerminkan hasil akhir approval ke TRX_Leave_Request.status.
// Batch D akan menambah di sini: potong/kembalikan TRX_Leave_Balance saat Approved/Cancelled.
public class LeaveApprovalHandler : IApprovalHandler
{
    private readonly ILeaveRequestRepository _leaveRepository;
    private readonly IFlexibleBenefitRepository _benefitRepository;

    public LeaveApprovalHandler(ILeaveRequestRepository leaveRepository, IFlexibleBenefitRepository benefitRepository)
    {
        _leaveRepository = leaveRepository;
        _benefitRepository = benefitRepository;
    }

    public string RequestType => ApprovalRequestTypes.Leave;

    public async Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var leave = await _leaveRepository.GetByIdAsync(request.RequestRefId)
            ?? throw new NotFoundException($"LeaveRequest {request.RequestRefId} untuk approval {request.Id} tidak ditemukan.");

        if (finalStatus == ApprovalRequestStatus.Approved)
        {
            var parts = new List<(int Year, DateOnly Start, DateOnly End, int Days)>();
            for (var year = leave.StartDate.Year; year <= leave.EndDate.Year; year++)
            {
                var start = year == leave.StartDate.Year ? leave.StartDate : new DateOnly(year, 1, 1);
                var end = year == leave.EndDate.Year ? leave.EndDate : new DateOnly(year, 12, 31);
                parts.Add((year, start, end, await CountWorkingDaysAsync(start, end)));
            }

            foreach (var part in parts)
            {
                var balance = await _benefitRepository.GetBalanceAsync(leave.EmployeeId, leave.LeaveTypeId, part.Year)
                    ?? throw new BadRequestException($"Saldo cuti tahun {part.Year} tidak ditemukan.");
                var available = balance.Entitlement + balance.CarriedOver - balance.Used - balance.Sold;
                if (available < part.Days)
                    throw new BadRequestException($"Saldo cuti tidak cukup saat approval. Tersedia {Math.Max(available, 0)} hari.");
                balance.Used += part.Days;
            }
        }

        leave.Status = finalStatus switch
        {
            ApprovalRequestStatus.Approved => LeaveRequestStatus.Approved,
            ApprovalRequestStatus.Rejected => LeaveRequestStatus.Rejected,
            ApprovalRequestStatus.Cancelled => LeaveRequestStatus.Cancelled,
            _ => leave.Status
        };

        await _benefitRepository.SaveAsync();
        await _leaveRepository.UpdateAsync(leave);
    }

    private async Task<int> CountWorkingDaysAsync(DateOnly start, DateOnly end)
    {
        var holidays = await _benefitRepository.GetHolidaysAsync(start, end);
        var set = holidays.Select(x => x.Date).ToHashSet();
        var count = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
            if (d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday && !set.Contains(d))
                count++;
        return count;
    }
}
