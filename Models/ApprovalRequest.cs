using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// TRX_Approval_Request. Satu baris per pengajuan dari modul mana pun.
// request_ref_id menunjuk ke baris modul asal (mis. TRX_Leave_Request.leave_id) tanpa FK,
// karena tabel asalnya berbeda-beda; yang menjamin konsistensi adalah unique (request_type, request_ref_id).
public class ApprovalRequest : IAuditable
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string RequestType { get; set; } = string.Empty;

    public int RequestRefId { get; set; }

    public int RequesterId { get; set; }
    public Employee? Requester { get; set; }

    // Ringkasan singkat untuk inbox & notifikasi, mis. "Cuti 12 Okt - 14 Okt 2026 (3 hari kerja)".
    [MaxLength(300)]
    public string? Summary { get; set; }

    public ApprovalRequestStatus Status { get; set; } = ApprovalRequestStatus.Pending;

    // Level langkah yang sedang ditunggu.
    public int CurrentLevel { get; set; } = 1;

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    public ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
}
