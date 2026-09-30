using HRIS.Api.DTOs.Employee;

namespace HRIS.Api.Services;

public interface IEmployeeService
{
    Task<List<EmployeeResponseDto>> GetAllAsync();
    Task<EmployeeResponseDto> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto);
    Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<EmployeeResponseDto> ChangeEmploymentAsync(int id, EmploymentChangeDto dto);
    Task<EmployeeResponseDto> ChangeDirectManagerAsync(int id, ChangeManagerDto dto);
    Task DeleteAsync(int id);
}
