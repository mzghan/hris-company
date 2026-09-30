namespace HRIS.Api.DTOs.Approval;

// Yang boleh diubah HR tanpa deploy: kedalaman rantai, ambang hari, dan aktif/tidaknya langkah.
public class ApprovalFlowStepUpdateDto
{
    public int? ChainDepth { get; set; }
    public int? MinRequestedDays { get; set; }
    public bool IsActive { get; set; } = true;
}
