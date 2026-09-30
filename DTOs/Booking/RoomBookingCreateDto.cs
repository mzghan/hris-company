using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Booking;
public class RoomBookingCreateDto
{
    [Required] public int RoomId { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Required] public DateTime StartTime { get; set; }
    [Required] public DateTime EndTime { get; set; }
    public bool IsPriority { get; set; }
}
