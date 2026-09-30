using HRIS.Api.Common;
using HRIS.Api.DTOs.Regulation;

namespace HRIS.Api.Services;

public interface IRegulationService
{
    Task<List<RegulationResponseDto>> GetAllAsync(bool includeInactive, UserContext actor);
    Task<RegulationResponseDto> CreateAsync(RegulationCreateDto dto, UserContext actor);
    Task<RegulationResponseDto> UpdateAsync(int id, RegulationCreateDto dto, UserContext actor);
    Task DeleteAsync(int id, UserContext actor);
}
