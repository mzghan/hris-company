using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IPerformanceRepository
{
    Task<List<PerformancePlan>> GetPlansAsync(int? employeeId);
    Task<PerformancePlan?> GetPlanAsync(int id);
    Task<PerformancePlan> AddPlanAsync(PerformancePlan x);
    Task<PlanTask> AddTaskAsync(PlanTask x);
    Task<PlanWorkLog> AddWorkLogAsync(PlanWorkLog x);
    Task UpdatePlanAsync(PerformancePlan x);
    Task UpdateTaskAsync(PlanTask x);
}