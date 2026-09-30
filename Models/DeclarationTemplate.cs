using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Declaration_Template. Konten template dapat disetujui/diperbarui HR tanpa migration.
public class DeclarationTemplate : IAuditable
{
    public int Id { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Required] public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
