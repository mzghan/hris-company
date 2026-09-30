using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Approval;

public class ApprovalActionDto
{
    // Opsional saat approve, wajib saat reject (divalidasi di ApprovalService).
    [MaxLength(500)]
    public string? Note { get; set; }
}
