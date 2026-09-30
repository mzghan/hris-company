using HRIS.Api.Common;
using HRIS.Api.DTOs.Leave;

namespace HRIS.Api.Services;

// Approve/Reject tidak ada di sini lagi: keputusan approver lewat IApprovalService
// (POST /api/approvals/{id}/approve|reject), dan hasilnya dicerminkan ke LeaveRequest oleh LeaveApprovalHandler.
public interface ILeaveRequestService
{
    Task<LeaveRequestResponseDto> CreateAsync(int employeeId, LeaveRequestCreateDto dto);
    Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId);
    Task<LeaveRequestResponseDto> CancelAsync(int leaveRequestId, UserContext actor);
}
