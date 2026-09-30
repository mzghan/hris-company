using HRIS.Api.DTOs.Approval;

namespace HRIS.Api.DTOs.Leave;

public class LeaveRequestResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Days { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? ApprovalId { get; set; }
    public int CurrentLevel { get; set; }
    public List<ApprovalStepResponseDto> Approvals { get; set; } = new();
}
