using HRIS.Api.DTOs.Leave;

namespace HRIS.Api.Services;

public interface ILeaveRequestService
{
    Task<LeaveRequestResponseDto> CreateAsync(int employeeId, LeaveRequestCreateDto dto);
    Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId);
    Task<List<LeaveRequestResponseDto>> GetPendingForApproverAsync(int approverEmployeeId);
    Task<LeaveRequestResponseDto> ApproveAsync(int leaveRequestId, int approverEmployeeId, LeaveApprovalActionDto dto);
    Task<LeaveRequestResponseDto> RejectAsync(int leaveRequestId, int approverEmployeeId, LeaveApprovalActionDto dto);
}
