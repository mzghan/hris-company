using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface ILeaveRequestRepository
{
    Task<List<LeaveRequest>> GetByEmployeeAsync(int employeeId);
    Task<LeaveRequest?> GetByIdAsync(int id);

    // Leave request yang sedang menunggu approval dari approverId tertentu
    // (dicocokkan lewat LeaveApproval.ApproverId + CurrentLevel).
    Task<List<LeaveRequest>> GetPendingForApproverAsync(int approverEmployeeId);

    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task UpdateAsync(LeaveRequest leaveRequest);
}
