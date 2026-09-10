using HRIS.Api.DTOs.Payroll;

namespace HRIS.Api.Services;

public interface IEmployeeSalaryService
{
    Task<EmployeeSalaryResponseDto> CreateAsync(EmployeeSalaryCreateDto dto);
    Task<List<EmployeeSalaryResponseDto>> GetHistoryAsync(int employeeId);
}
