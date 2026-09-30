using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class ManpowerRepository : IManpowerRepository
{
    private readonly AppDbContext _context;
    public ManpowerRepository(AppDbContext context)=>_context=context;
    private IQueryable<ManpowerRequest> Base()=>_context.ManpowerRequests.Include(x=>x.RequestedByEmployee).Include(x=>x.Organization).Include(x=>x.JobTitle).Include(x=>x.JobLevel).Include(x=>x.EmploymentType);
    public Task<List<ManpowerRequest>> GetAllAsync(int? employeeId=null)=>Base().Where(x=>employeeId==null||x.RequestedByEmployeeId==employeeId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    public Task<ManpowerRequest?> GetByIdAsync(int id)=>Base().FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<ManpowerRequest> AddAsync(ManpowerRequest x){_context.ManpowerRequests.Add(x);await _context.SaveChangesAsync();return x;}
    public async Task UpdateAsync(ManpowerRequest x)=>await _context.SaveChangesAsync();
}
