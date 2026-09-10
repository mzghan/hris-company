using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class EmployeeSalaryService : IEmployeeSalaryService
{
    private readonly IEmployeeSalaryRepository _salaryRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeSalaryService> _logger;

    public EmployeeSalaryService(
        IEmployeeSalaryRepository salaryRepository,
        IEmployeeRepository employeeRepository,
        ILogger<EmployeeSalaryService> logger)
    {
        _salaryRepository = salaryRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<EmployeeSalaryResponseDto> CreateAsync(EmployeeSalaryCreateDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId)
            ?? throw new NotFoundException($"Employee dengan id {dto.EmployeeId} tidak ditemukan.");

        var salary = new EmployeeSalary
        {
            EmployeeId = dto.EmployeeId,
            BaseSalary = dto.BaseSalary,
            AllowanceTotal = dto.AllowanceTotal,
            EffectiveDate = dto.EffectiveDate
        };

        await _salaryRepository.AddAsync(salary);
        _logger.LogInformation(
            "EmployeeSalary baru dibuat untuk employee {EmployeeId}, berlaku mulai {EffectiveDate}",
            dto.EmployeeId, dto.EffectiveDate);

        return ToDto(salary, employee.FullName);
    }

    public async Task<List<EmployeeSalaryResponseDto>> GetHistoryAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee dengan id {employeeId} tidak ditemukan.");

        var history = await _salaryRepository.GetByEmployeeAsync(employeeId);
        return history.Select(s => ToDto(s, employee.FullName)).ToList();
    }

    private static EmployeeSalaryResponseDto ToDto(EmployeeSalary s, string employeeName) => new()
    {
        Id = s.Id,
        EmployeeId = s.EmployeeId,
        EmployeeName = employeeName,
        BaseSalary = s.BaseSalary,
        AllowanceTotal = s.AllowanceTotal,
        EffectiveDate = s.EffectiveDate,
        CreatedAt = s.CreatedAt
    };
}
