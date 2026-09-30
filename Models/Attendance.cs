namespace HRIS.Api.Models;

public class Attendance
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly Date { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }

    public int WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }
    public string? Note { get; set; }

    // --- Bukti check-in: foto dari kamera + koordinat GPS ---
    public string? CheckInPhotoPath { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }

    // --- Bukti check-out: foto dari kamera + koordinat GPS ---
    public string? CheckOutPhotoPath { get; set; }
    public double? CheckOutLatitude { get; set; }
    public double? CheckOutLongitude { get; set; }
}
