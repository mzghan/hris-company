using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class AssistanceRequestRepository : IAssistanceRequestRepository
{
    private readonly AppDbContext _context;
    public AssistanceRequestRepository(AppDbContext context) => _context = context;

    private IQueryable<AssistanceRequest> BaseQuery() => _context.AssistanceRequests
        .Include(x => x.Employee)
        .Include(x => x.HandledByUser).ThenInclude(x => x!.Employee);

    public Task<List<AssistanceRequest>> GetAllAsync(bool includeAnonymous) => BaseQuery()
        .Where(x => includeAnonymous || !x.IsAnonymous)
        .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        .ToListAsync();

    public Task<List<AssistanceRequest>> GetByEmployeeAsync(int employeeId) => BaseQuery()
        .Where(x => x.EmployeeId == employeeId && !x.IsAnonymous)
        .OrderByDescending(x => x.CreatedAt).ToListAsync();

    public Task<AssistanceRequest?> GetByIdAsync(int id) => BaseQuery().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<AssistanceRequest> AddAsync(AssistanceRequest request)
    {
        _context.AssistanceRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task UpdateAsync(AssistanceRequest request) => await _context.SaveChangesAsync();
}
