using HRIS.Api.Common; using HRIS.Api.DTOs.Family;
namespace HRIS.Api.Services; public interface IFamilyChangeService { Task<List<FamilyChangeResponseDto>> GetAsync(UserContext actor); Task<FamilyChangeResponseDto> CreateAsync(FamilyChangeCreateDto dto, UserContext actor); Task ApplyApprovedAsync(int id); }
