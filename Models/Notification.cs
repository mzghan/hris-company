using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Notification. Notifikasi in-app per user.
public class Notification : IAuditable
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    // Halaman tujuan saat notifikasi diklik (path relatif, mis. "/Approvals/Index").
    [MaxLength(300)]
    public string? LinkUrl { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
