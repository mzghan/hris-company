namespace HRIS.Api.DTOs.Attendance;

public class AttendanceCheckInDto
{
    public string? PhotoBase64 { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int WorkTypeId { get; set; }
    public string? Note { get; set; }
}
