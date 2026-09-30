using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// SYS_Audit_Log. Jejak aksi sensitif, terutama aksi Support yang melewati pengecekan
// kepemilikan data (keputusan 3 di dokumen desain).
public class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    [Required, MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }

    [MaxLength(500)]
    public string? Detail { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
