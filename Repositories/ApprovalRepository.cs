using HRIS.Api.Data;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class ApprovalRepository : IApprovalRepository
{
    private readonly AppDbContext _context;

    public ApprovalRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<ApprovalRequest> BaseQuery() =>
        _context.ApprovalRequests
            .Include(r => r.Requester)
            .Include(r => r.Steps).ThenInclude(s => s.ApproverRole)
            .Include(r => r.Steps).ThenInclude(s => s.ApproverEmployee)
            .Include(r => r.Steps).ThenInclude(s => s.ActedByUser)
            .AsSplitQuery();

    public async Task<ApprovalRequest?> GetByIdAsync(int id) =>
        await BaseQuery().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<ApprovalRequest?> GetByRefAsync(string requestType, int requestRefId) =>
        await BaseQuery().FirstOrDefaultAsync(r => r.RequestType == requestType && r.RequestRefId == requestRefId);

    public async Task<List<ApprovalRequest>> GetByRefsAsync(string requestType, IEnumerable<int> requestRefIds)
    {
        var ids = requestRefIds.ToList();
        return await BaseQuery()
            .Where(r => r.RequestType == requestType && ids.Contains(r.RequestRefId))
            .ToListAsync();
    }

    public async Task<List<ApprovalRequest>> GetByRequesterAsync(int requesterEmployeeId) =>
        await BaseQuery()
            .Where(r => r.RequesterId == requesterEmployeeId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<List<ApprovalRequest>> GetInboxAsync(int? employeeId, IEnumerable<string> roleNames)
    {
        var roles = roleNames.ToList();
        // -1 tidak pernah cocok dengan Id mana pun, jadi akun tanpa Employee (mis. Support awal)
        // hanya bisa menerima langkah bertipe Role.
        var empId = employeeId ?? -1;

        return await BaseQuery()
            .Where(r => r.Status == ApprovalRequestStatus.Pending
                        && r.RequesterId != empId
                        && r.Steps.Any(s => s.Level == r.CurrentLevel
                                            && s.Status == ApprovalStepStatus.Pending
                                            && ((s.ApproverType == ApproverType.Employee && s.ApproverEmployeeId == empId)
                                                || (s.ApproverType == ApproverType.Role && roles.Contains(s.ApproverRole!.RoleName)))))
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ApprovalRequest>> GetAllPendingAsync() =>
        await BaseQuery()
            .Where(r => r.Status == ApprovalRequestStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

    public async Task<ApprovalRequest> AddAsync(ApprovalRequest request)
    {
        _context.ApprovalRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task UpdateAsync(ApprovalRequest request)
    {
        // Entity sudah tracked; tidak memakai Update() supaya Requester/Role/User di graph tidak ikut ditandai Modified.
        await _context.SaveChangesAsync();
    }

    public async Task<List<ApprovalFlowStep>> GetActiveFlowStepsAsync(string requestType) =>
        await _context.ApprovalFlowSteps
            .Where(f => f.RequestType == requestType && f.IsActive)
            .OrderBy(f => f.Level)
            .ToListAsync();

    public async Task<List<ApprovalFlowStep>> GetAllFlowStepsAsync() =>
        await _context.ApprovalFlowSteps
            .Include(f => f.Role)
            .Include(f => f.ApproverEmployee)
            .OrderBy(f => f.RequestType).ThenBy(f => f.Level)
            .ToListAsync();

    public async Task<ApprovalFlowStep?> GetFlowStepByIdAsync(int id) =>
        await _context.ApprovalFlowSteps
            .Include(f => f.Role)
            .Include(f => f.ApproverEmployee)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task UpdateFlowStepAsync(ApprovalFlowStep step)
    {
        _context.ApprovalFlowSteps.Update(step);
        await _context.SaveChangesAsync();
    }
}
