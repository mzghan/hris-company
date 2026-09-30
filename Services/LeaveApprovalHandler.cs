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

    public LeaveApprovalHandler(ILeaveRequestRepository leaveRepository)
    {
        _leaveRepository = leaveRepository;
    }

    public string RequestType => ApprovalRequestTypes.Leave;

    public async Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var leave = await _leaveRepository.GetByIdAsync(request.RequestRefId)
            ?? throw new NotFoundException($"LeaveRequest {request.RequestRefId} untuk approval {request.Id} tidak ditemukan.");

        leave.Status = finalStatus switch
        {
            ApprovalRequestStatus.Approved => LeaveRequestStatus.Approved,
            ApprovalRequestStatus.Rejected => LeaveRequestStatus.Rejected,
            ApprovalRequestStatus.Cancelled => LeaveRequestStatus.Cancelled,
            _ => leave.Status
        };

        await _leaveRepository.UpdateAsync(leave);
    }
}
