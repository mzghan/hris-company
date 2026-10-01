using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface ICompetencyRepository
{
    Task<List<Competency>> GetAllAsync();
    Task<Competency?> GetByIdAsync(int id);
    Task<List<EmployeeCompetency>> GetEmployeeCompetenciesAsync(int? employeeId);
    Task<EmployeeCompetency?> GetEmployeeCompetencyAsync(int id);
    Task<EmployeeCompetency> AddEmployeeCompetencyAsync(EmployeeCompetency x);
    Task<CompetencyAssessment> AddAssessmentAsync(CompetencyAssessment x);
}