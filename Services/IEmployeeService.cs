using HRIS.Api.DTOs.Employee;

namespace HRIS.Api.Services;

public interface IEmployeeService
{
    Task<List<EmployeeResponseDto>> GetAllAsync();
    Task<EmployeeResponseDto> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto);
    Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task DeleteAsync(int id);
}
