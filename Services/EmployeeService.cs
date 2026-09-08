using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        return employees.Select(ToDto).ToList();
    }

    public async Task<EmployeeResponseDto> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");
        return ToDto(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto)
    {
        var existing = await _repository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new BadRequestException($"Email {dto.Email} sudah dipakai employee lain.");

        if (dto.ManagerId is not null)
        {
            var manager = await _repository.GetByIdAsync(dto.ManagerId.Value)
                ?? throw new BadRequestException($"ManagerId {dto.ManagerId} tidak ditemukan.");
        }

        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Position = dto.Position,
            HireDate = dto.HireDate,
            DepartmentId = dto.DepartmentId,
            ManagerId = dto.ManagerId
        };

        await _repository.AddAsync(employee);
        _logger.LogInformation("Employee {Name} dibuat dengan id {Id}", employee.FullName, employee.Id);

        var created = await _repository.GetByIdAsync(employee.Id);
        return ToDto(created!);
    }

    public async Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

        if (dto.ManagerId == id)
            throw new BadRequestException("Employee tidak bisa menjadi manager untuk dirinya sendiri.");

        employee.FullName = dto.FullName;
        employee.Position = dto.Position;
        employee.DepartmentId = dto.DepartmentId;
        employee.ManagerId = dto.ManagerId;
        employee.IsActive = dto.IsActive;

        await _repository.UpdateAsync(employee);

        var updated = await _repository.GetByIdAsync(id);
        return ToDto(updated!);
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

        await _repository.DeleteAsync(employee);
    }

    private static EmployeeResponseDto ToDto(Employee e) => new()
    {
        Id = e.Id,
        FullName = e.FullName,
        Email = e.Email,
        Position = e.Position,
        HireDate = e.HireDate,
        IsActive = e.IsActive,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name,
        ManagerId = e.ManagerId,
        ManagerName = e.Manager?.FullName
    };
}
