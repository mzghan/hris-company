using HRIS.Api.DTOs.Organization;

namespace HRIS.Api.Services;

public interface IOrganizationService
{
    Task<List<OrganizationResponseDto>> GetAllAsync();
    Task<OrganizationResponseDto> GetByIdAsync(int id);
    Task<OrganizationResponseDto> CreateAsync(OrganizationCreateDto dto);
    Task<OrganizationResponseDto> UpdateAsync(int id, OrganizationCreateDto dto);
    Task DeleteAsync(int id);
}
