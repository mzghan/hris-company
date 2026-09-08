using HRIS.Api.Data;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly AppDbContext _context;

    public LeaveRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<LeaveRequest> BaseQuery() =>
        _context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.Approvals)
                .ThenInclude(a => a.Approver);

    public async Task<List<LeaveRequest>> GetByEmployeeAsync(int employeeId) =>
        await BaseQuery()
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<LeaveRequest?> GetByIdAsync(int id) =>
        await BaseQuery().FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<LeaveRequest>> GetPendingForApproverAsync(int approverEmployeeId) =>
        await BaseQuery()
            .Where(l => l.Status == LeaveRequestStatus.Pending &&
                        l.Approvals.Any(a => a.ApproverId == approverEmployeeId &&
                                              a.Level == l.CurrentLevel &&
                                              a.Status == ApprovalStatus.Pending))
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();

    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task UpdateAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Update(leaveRequest);
        await _context.SaveChangesAsync();
    }
}
