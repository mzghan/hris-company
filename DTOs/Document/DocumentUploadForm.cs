using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Document;

// Form multipart untuk POST /api/documents.
public class DocumentUploadForm
{
    [Required]
    public IFormFile? File { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    // Kosong = dokumen umum. Terisi = dokumen pribadi milik karyawan tsb.
    public int? OwnerEmployeeId { get; set; }
}
