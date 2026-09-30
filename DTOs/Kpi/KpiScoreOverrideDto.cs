using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Kpi;

public class KpiScoreOverrideDto
{
    [Range(0, 100)]
    public decimal NewScore { get; set; }

    // Wajib diisi (beda dari LeaveApprovalActionDto.Note
    // yang opsional) — override nilai KPI butuh justifikasi, lihat bab 10.2 poin 4
    // technical alignment doc.
    [Required, MaxLength(500)]
    public string Note { get; set; } = string.Empty;
}
