namespace HRIS.Api.DTOs.Attendance;

public class AttendanceResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }

    public string? CheckInPhotoUrl { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }

    public string? CheckOutPhotoUrl { get; set; }
    public double? CheckOutLatitude { get; set; }
    public double? CheckOutLongitude { get; set; }
}
