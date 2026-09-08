using HRIS.Api.DTOs.Attendance;

namespace HRIS.Api.Services;

public interface IAttendanceService
{
    Task<AttendanceResponseDto> CheckInAsync(int employeeId);
    Task<AttendanceResponseDto> CheckOutAsync(int employeeId);
    Task<List<AttendanceResponseDto>> GetHistoryAsync(int employeeId);
}
