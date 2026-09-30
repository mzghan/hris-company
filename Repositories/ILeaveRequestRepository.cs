using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface ILeaveRequestRepository
{
    Task<List<LeaveRequest>> GetByEmployeeAsync(int employeeId);
    Task<LeaveRequest?> GetByIdAsync(int id);

    // Ada pengajuan Pending/Approved milik employee yang tanggalnya bertabrakan?
    Task<bool> HasOverlapAsync(int employeeId, DateOnly startDate, DateOnly endDate);

    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task UpdateAsync(LeaveRequest leaveRequest);
}
