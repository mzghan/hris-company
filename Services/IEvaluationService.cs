using HRIS.Api.Common;
using HRIS.Api.DTOs.Evaluation;
namespace HRIS.Api.Services;
public interface IEvaluationService
{
 Task<List<EvaluationResponseDto>> GetAsync(UserContext actor);
 Task<EvaluationResponseDto> GetByIdAsync(int id, UserContext actor);
 Task<EvaluationResponseDto> AddEntryAsync(int id, EvaluationEntryCreateDto dto, UserContext actor);
 Task<EvaluationResponseDto> AddScoreAsync(int id, EvaluationScoreCreateDto dto, UserContext actor);
}