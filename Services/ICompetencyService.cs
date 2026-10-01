using HRIS.Api.Common;
using HRIS.Api.DTOs.Competency;
namespace HRIS.Api.Services;
public interface ICompetencyService
{
 Task<List<CompetencyResponseDto>> GetCompetenciesAsync();
 Task<List<EmployeeCompetencyResponseDto>> GetEmployeeCompetenciesAsync(UserContext actor);
 Task<EmployeeCompetencyResponseDto> AssignAsync(EmployeeCompetencyCreateDto dto,UserContext actor);
 Task<EmployeeCompetencyResponseDto> AssessAsync(CompetencyAssessmentCreateDto dto,UserContext actor);
}