using HRIS.Api.Common;
using HRIS.Api.DTOs.PersonalAction;
namespace HRIS.Api.Services;
public interface IPersonalActionService
{
 Task<List<PersonalActionResponseDto>> GetAsync(UserContext actor);
 Task<PersonalActionResponseDto> CreateAsync(PersonalActionCreateDto dto,UserContext actor);
}