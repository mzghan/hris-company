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

    // Approver & langkahnya tidak lagi di-include di sini: ada di approval engine (IApprovalService).
    private IQueryable<LeaveRequest> BaseQuery() =>
        _context.LeaveRequests.Include(l => l.Employee);

    public async Task<List<LeaveRequest>> GetByEmployeeAsync(int employeeId) =>
        await BaseQuery()
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<LeaveRequest?> GetByIdAsync(int id) =>
        await BaseQuery().FirstOrDefaultAsync(l => l.Id == id);

    public async Task<bool> HasOverlapAsync(int employeeId, DateOnly startDate, DateOnly endDate) =>
        await _context.LeaveRequests.AnyAsync(l =>
            l.EmployeeId == employeeId
            && (l.Status == LeaveRequestStatus.Pending || l.Status == LeaveRequestStatus.Approved)
            && l.StartDate <= endDate
            && l.EndDate >= startDate);

    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task UpdateAsync(LeaveRequest leaveRequest)
    {
        // Entity sudah tracked; Update() akan ikut menandai Employee (include) sebagai Modified.
        await _context.SaveChangesAsync();
    }
}
