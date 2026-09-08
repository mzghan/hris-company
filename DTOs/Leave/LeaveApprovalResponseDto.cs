namespace HRIS.Api.DTOs.Leave;

public class LeaveApprovalResponseDto
{
    public int Id { get; set; }
    public int Level { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ApproverId { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime? ActedAt { get; set; }
}
