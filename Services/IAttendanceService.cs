using HRIS.Api.DTOs.Attendance;

namespace HRIS.Api.Services;

public interface IAttendanceService
{
    Task<AttendanceResponseDto> CheckInAsync(int employeeId, AttendanceCheckInDto dto);
    Task<AttendanceResponseDto> CheckOutAsync(int employeeId, AttendanceCheckOutDto dto);
    Task<List<AttendanceResponseDto>> GetHistoryAsync(int employeeId);
}
