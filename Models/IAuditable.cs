namespace HRIS.Api.Models;

// Kolom audit standar (poin 3.4 di dokumen desain DB). Diisi otomatis
// oleh AppDbContext.SaveChangesAsync, jadi tidak perlu diisi manual
// di Service/Repository.
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    int? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    int? UpdatedBy { get; set; }
}
