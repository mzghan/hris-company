using HRIS.Api.DTOs.Department;

namespace HRIS.Api.Services;

public interface IDepartmentService
{
    Task<List<DepartmentResponseDto>> GetAllAsync();
    Task<DepartmentResponseDto> GetByIdAsync(int id);
    Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto);
    Task<DepartmentResponseDto> UpdateAsync(int id, DepartmentCreateDto dto);
    Task DeleteAsync(int id);
}
