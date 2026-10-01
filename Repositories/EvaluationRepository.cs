using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class EvaluationRepository : IEvaluationRepository
{
    private readonly AppDbContext _db;
    public EvaluationRepository(AppDbContext db)=>_db=db;
    private IQueryable<EmployeeEvaluation> Base()=>_db.EmployeeEvaluations
        .Include(x=>x.Employee).Include(x=>x.Employment)
        .Include(x=>x.EvaluationType)
        .Include(x=>x.Entries).Include(x=>x.Scores).ThenInclude(x=>x.ScoredByUser);
    public Task<List<EmployeeEvaluation>> GetAllAsync(int? employeeId)=>Base().Where(x=>employeeId==null||x.EmployeeId==employeeId).OrderBy(x=>x.DueDate).ToListAsync();
    public Task<EmployeeEvaluation?> GetByIdAsync(int id)=>Base().FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<EmployeeEvaluation> AddAsync(EmployeeEvaluation x){_db.EmployeeEvaluations.Add(x);await _db.SaveChangesAsync();return x;}
    public Task<List<EvaluationType>> GetTypesAsync()=>_db.EvaluationTypes.OrderBy(x=>x.Id).ToListAsync();
    public Task<List<EmployeeEmployment>> GetActiveEmploymentsAsync()=>_db.EmployeeEmployments.Include(x=>x.Employee).Include(x=>x.EmploymentStatus).Include(x=>x.EmploymentType).Where(x=>x.EndDate==null&&x.Employee!.IsActive).ToListAsync();
    public async Task<EvaluationEntry> AddEntryAsync(EvaluationEntry x){_db.EvaluationEntries.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task<EvaluationScore> AddScoreAsync(EvaluationScore x){_db.EvaluationScores.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task UpdateAsync(EmployeeEvaluation x)=>await _db.SaveChangesAsync();
}