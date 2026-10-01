using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class CompetencyRepository : ICompetencyRepository
{
    private readonly AppDbContext _db;
    public CompetencyRepository(AppDbContext db)=>_db=db;
    public Task<List<Competency>> GetAllAsync()=>_db.Competencies.OrderBy(x=>x.Name).ToListAsync();
    public Task<Competency?> GetByIdAsync(int id)=>_db.Competencies.FirstOrDefaultAsync(x=>x.Id==id);
    public Task<List<EmployeeCompetency>> GetEmployeeCompetenciesAsync(int? employeeId)=>_db.EmployeeCompetencies
        .Include(x=>x.Employee).Include(x=>x.Competency).Include(x=>x.AssignedByUser)
        .Include(x=>x.Assessments).Where(x=>employeeId==null||x.EmployeeId==employeeId).OrderBy(x=>x.Employee!.FullName).ThenBy(x=>x.Competency!.Name).ToListAsync();
    public Task<EmployeeCompetency?> GetEmployeeCompetencyAsync(int id)=>_db.EmployeeCompetencies.Include(x=>x.Employee).Include(x=>x.Competency).Include(x=>x.Assessments).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<EmployeeCompetency> AddEmployeeCompetencyAsync(EmployeeCompetency x){_db.EmployeeCompetencies.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task<CompetencyAssessment> AddAssessmentAsync(CompetencyAssessment x){_db.CompetencyAssessments.Add(x);await _db.SaveChangesAsync();return x;}
}