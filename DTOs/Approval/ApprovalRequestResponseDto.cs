namespace HRIS.Api.DTOs.Approval;

public class ApprovalRequestResponseDto
{
    public int Id { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string RequestTypeLabel { get; set; } = string.Empty;
    public int RequestRefId { get; set; }
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CurrentLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // True kalau user yang sedang login boleh Approve/Reject langkah aktif saat ini.
    public bool CanAct { get; set; }

    public List<ApprovalStepResponseDto> Steps { get; set; } = new();
}
