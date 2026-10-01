using HRIS.Api.Common;
using HRIS.Api.DTOs.Performance;
namespace HRIS.Api.Services;
public interface IPerformanceService
{
 Task<List<PerformancePlanResponseDto>> GetPlansAsync(UserContext actor);
 Task<PerformancePlanResponseDto> CreatePlanAsync(PerformancePlanCreateDto dto,UserContext actor);
 Task<PerformancePlanResponseDto> AddTaskAsync(PlanTaskCreateDto dto,UserContext actor);
 Task<PerformancePlanResponseDto> AddWorkLogAsync(PlanWorkLogCreateDto dto,UserContext actor);
 Task<PerformancePlanResponseDto> UpdateTaskStatusAsync(int id,PlanTaskStatusDto dto,UserContext actor);
}