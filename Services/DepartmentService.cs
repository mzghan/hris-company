using HRIS.Api.DTOs.Department;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository repository, ILogger<DepartmentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<DepartmentResponseDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();
        return departments.Select(ToDto).ToList();
    }

    public async Task<DepartmentResponseDto> GetByIdAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Department dengan id {id} tidak ditemukan.");
        return ToDto(department);
    }

    public async Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto)
    {
        var department = new Department { Name = dto.Name };
        await _repository.AddAsync(department);
        _logger.LogInformation("Department {Name} dibuat dengan id {Id}", department.Name, department.Id);
        return ToDto(department);
    }

    public async Task<DepartmentResponseDto> UpdateAsync(int id, DepartmentCreateDto dto)
    {
        var department = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Department dengan id {id} tidak ditemukan.");

        department.Name = dto.Name;
        await _repository.UpdateAsync(department);
        return ToDto(department);
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Department dengan id {id} tidak ditemukan.");

        if (department.Employees.Any())
            throw new BadRequestException("Department masih memiliki employee, pindahkan dulu sebelum menghapus.");

        await _repository.DeleteAsync(department);
    }

    private static DepartmentResponseDto ToDto(Department d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        EmployeeCount = d.Employees?.Count ?? 0
    };
}
