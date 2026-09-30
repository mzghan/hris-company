using HRIS.Api.Common;
using HRIS.Api.DTOs.Leave;
using HRIS.Api.Models;

namespace HRIS.Api.Services;

// Approve/Reject tidak ada di sini lagi: keputusan approver lewat IApprovalService
// (POST /api/approvals/{id}/approve|reject), dan hasilnya dicerminkan ke LeaveRequest oleh LeaveApprovalHandler.
public interface ILeaveRequestService
{
    Task<LeaveRequestResponseDto> CreateAsync(int employeeId, LeaveRequestCreateDto dto);
    Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId);
    Task<LeaveRequestResponseDto> CancelAsync(int leaveRequestId, UserContext actor);
    Task<List<LeaveBalance>> GetBalancesAsync(int employeeId, int year);
    Task<List<LeaveType>> GetLeaveTypesAsync();
    Task<int> CountWorkingDaysAsync(DateOnly start, DateOnly end);
}
