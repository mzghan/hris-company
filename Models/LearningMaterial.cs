using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Learning_Material. Materi pembelajaran selalu menunjuk ke dokumen di TRX_Document.
public class LearningMaterial : IAuditable
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int DocumentId { get; set; }
    public Document? Document { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
