namespace HRIS.Api.DTOs.Leave;

public class LeaveRequestResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CurrentLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<LeaveApprovalResponseDto> Approvals { get; set; } = new();
}
