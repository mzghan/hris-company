using HRIS.Api.Common;
using HRIS.Api.DTOs.Assistance;

namespace HRIS.Api.Services;

public interface IAssistanceRequestService
{
    Task<List<AssistanceRequestResponseDto>> GetListAsync(UserContext actor);
    Task<AssistanceRequestResponseDto> CreateAsync(AssistanceRequestCreateDto dto, UserContext actor);
    Task<AssistanceRequestResponseDto> UpdateStatusAsync(int id, string status, UserContext actor);
}
