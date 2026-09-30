using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class RoomBooking : IAuditable
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public MeetingRoom? Room { get; set; }
    public int BookedByEmployeeId { get; set; }
    public Employee? BookedByEmployee { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Confirmed";
    public bool IsPriority { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
