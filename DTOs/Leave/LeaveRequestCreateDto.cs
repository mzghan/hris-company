using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Leave;

public class LeaveRequestCreateDto
{
    [Required] public int LeaveTypeId { get; set; }
    [Required] public DateOnly StartDate { get; set; }
    [Required] public DateOnly EndDate { get; set; }
    [MaxLength(500)] public string? Reason { get; set; }
}
