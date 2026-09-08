using HRIS.Api.DTOs.Attendance;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<AttendanceService> _logger;

    public AttendanceService(
        IAttendanceRepository repository,
        IEmployeeRepository employeeRepository,
        ILogger<AttendanceService> logger)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<AttendanceResponseDto> CheckInAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException("Employee tidak ditemukan.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await _repository.GetByEmployeeAndDateAsync(employeeId, today);

        if (existing is not null)
            throw new BadRequestException("Sudah check-in hari ini.");

        var attendance = new Attendance
        {
            EmployeeId = employeeId,
            Date = today,
            CheckIn = DateTime.UtcNow.TimeOfDay
        };

        await _repository.AddAsync(attendance);
        _logger.LogInformation("Employee {Id} check-in pada {Date}", employeeId, today);

        attendance.Employee = employee;
        return ToDto(attendance);
    }

    public async Task<AttendanceResponseDto> CheckOutAsync(int employeeId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var attendance = await _repository.GetByEmployeeAndDateAsync(employeeId, today)
            ?? throw new BadRequestException("Belum check-in hari ini.");

        if (attendance.CheckOut is not null)
            throw new BadRequestException("Sudah check-out hari ini.");

        attendance.CheckOut = DateTime.UtcNow.TimeOfDay;
        await _repository.UpdateAsync(attendance);

        return ToDto(attendance);
    }

    public async Task<List<AttendanceResponseDto>> GetHistoryAsync(int employeeId)
    {
        var history = await _repository.GetByEmployeeAsync(employeeId);
        return history.Select(ToDto).ToList();
    }

    private static AttendanceResponseDto ToDto(Attendance a) => new()
    {
        Id = a.Id,
        EmployeeId = a.EmployeeId,
        EmployeeName = a.Employee?.FullName ?? string.Empty,
        Date = a.Date,
        CheckIn = a.CheckIn,
        CheckOut = a.CheckOut
    };
}
