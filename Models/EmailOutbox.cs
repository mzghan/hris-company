using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// TRX_Email_Outbox. Antrean email (outbox pattern): Service hanya menulis baris di sini
// dalam transaction yang sama dengan datanya, lalu background job yang mengirim.
// Jadi kegagalan SMTP tidak pernah menggagalkan proses bisnis.
public class EmailOutbox : IAuditable
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string ToAddress { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? ToName { get; set; }

    [Required, MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    public int Attempts { get; set; }

    [MaxLength(500)]
    public string? LastError { get; set; }

    // Percobaan berikutnya paling cepat kapan (backoff setelah gagal). Null = langsung.
    public DateTime? NextAttemptAt { get; set; }
    public DateTime? SentAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
