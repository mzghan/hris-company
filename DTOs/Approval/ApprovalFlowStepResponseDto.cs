namespace HRIS.Api.DTOs.Approval;

public class ApprovalFlowStepResponseDto
{
    public int Id { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string RequestTypeLabel { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ApproverType { get; set; } = string.Empty;
    public int? ChainDepth { get; set; }
    public string? RoleName { get; set; }
    public string? ApproverEmployeeName { get; set; }
    public int? MinRequestedDays { get; set; }
    public bool IsActive { get; set; }
}
