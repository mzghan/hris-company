using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Document. Metadata dokumen; file fisiknya ada di IFileStorage (bukan di wwwroot).
public class Document : IAuditable
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public DocumentCategory? Category { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    // Nama file asli saat diunggah (untuk nama download). Bukan nama file di disk.
    [Required, MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    // Path relatif di storage, dibuat oleh sistem (bukan input user).
    [Required, MaxLength(300)]
    public string StoredPath { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? ContentType { get; set; }

    public long SizeBytes { get; set; }

    // Null = dokumen umum (semua karyawan boleh baca, mis. form & materi).
    // Terisi = dokumen pribadi (surat jadi, deklarasi, dokumen ekspat): hanya pemilik, HR, Support.
    public int? OwnerEmployeeId { get; set; }
    public Employee? OwnerEmployee { get; set; }

    public int UploadedByUserId { get; set; }
    public User? UploadedByUser { get; set; }

    // Soft delete: file di disk tidak dihapus.
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
