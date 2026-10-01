using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class PerformanceRepository : IPerformanceRepository
{
    private readonly AppDbContext _db;
    public PerformanceRepository(AppDbContext db)=>_db=db;
    private IQueryable<PerformancePlan> Base()=>_db.PerformancePlans.Include(x=>x.Employee).Include(x=>x.CreatedByUser).Include(x=>x.Tasks).Include(x=>x.WorkLogs).AsSplitQuery();
    public Task<List<PerformancePlan>> GetPlansAsync(int? employeeId)=>Base().Where(x=>employeeId==null||x.EmployeeId==employeeId).OrderByDescending(x=>x.StartDate).ToListAsync();
    public Task<PerformancePlan?> GetPlanAsync(int id)=>Base().FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<PerformancePlan> AddPlanAsync(PerformancePlan x){_db.PerformancePlans.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task<PlanTask> AddTaskAsync(PlanTask x){_db.PlanTasks.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task<PlanWorkLog> AddWorkLogAsync(PlanWorkLog x){_db.PlanWorkLogs.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task UpdatePlanAsync(PerformancePlan x)=>await _db.SaveChangesAsync();
    public async Task UpdateTaskAsync(PlanTask x)=>await _db.SaveChangesAsync();
}