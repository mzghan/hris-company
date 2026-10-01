using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IEvaluationRepository
{
    Task<List<EmployeeEvaluation>> GetAllAsync(int? employeeId);
    Task<EmployeeEvaluation?> GetByIdAsync(int id);
    Task<EmployeeEvaluation> AddAsync(EmployeeEvaluation x);
    Task<List<EvaluationType>> GetTypesAsync();
    Task<List<EmployeeEmployment>> GetActiveEmploymentsAsync();
    Task<EvaluationEntry> AddEntryAsync(EvaluationEntry x);
    Task<EvaluationScore> AddScoreAsync(EvaluationScore x);
    Task UpdateAsync(EmployeeEvaluation x);
}