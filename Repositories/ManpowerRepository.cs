using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class ManpowerRepository : IManpowerRepository
{
    private readonly AppDbContext _context;
    public ManpowerRepository(AppDbContext context)=>_context=context;
    private IQueryable<ManpowerRequest> Base()=>_context.ManpowerRequests.Include(x=>x.RequestedByEmployee).Include(x=>x.Organization).Include(x=>x.JobTitle).Include(x=>x.JobLevel).Include(x=>x.EmploymentType).Include(x=>x.ReplacementForEmployee).Include(x=>x.ReportToEmployee);
    public Task<List<ManpowerRequest>> GetAllAsync(int? employeeId=null)=>Base().Where(x=>employeeId==null||x.RequestedByEmployeeId==employeeId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    public Task<ManpowerRequest?> GetByIdAsync(int id)=>Base().Include(x=>x.Remarks).Include(x=>x.Filings).Include(x=>x.Vacancies).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<ManpowerRequest> AddAsync(ManpowerRequest x){_context.ManpowerRequests.Add(x);await _context.SaveChangesAsync();return x;}
    public async Task UpdateAsync(ManpowerRequest x)=>await _context.SaveChangesAsync();
    public Task<List<ManpowerVacancy>> GetVacanciesAsync()=>_context.ManpowerVacancies.Include(x=>x.ManpowerRequest).ThenInclude(x=>x!.Organization).Include(x=>x.ManpowerRequest).ThenInclude(x=>x!.JobTitle).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    public async Task AddRemarkAsync(ManpowerRequestRemark remark){_context.ManpowerRequestRemarks.Add(remark);await _context.SaveChangesAsync();}
    public async Task AddFilingAsync(ManpowerRequestFiling filing){_context.ManpowerRequestFilings.Add(filing);await _context.SaveChangesAsync();}
}
