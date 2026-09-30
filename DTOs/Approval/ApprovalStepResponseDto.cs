namespace HRIS.Api.DTOs.Approval;

public class ApprovalStepResponseDto
{
    public int Id { get; set; }
    public int Level { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ApproverType { get; set; } = string.Empty;
    public int? ApproverEmployeeId { get; set; }

    // Nama karyawan (tipe Employee) atau "Role: HR" (tipe Role).
    public string ApproverName { get; set; } = string.Empty;

    public string? ActedByUsername { get; set; }
    public bool IsSupportOverride { get; set; }
    public string? Note { get; set; }
    public DateTime? ActedAt { get; set; }
}
