using HRIS.Api.Common;
using HRIS.Api.DTOs.Document;

namespace HRIS.Api.Services;

// Hasil OpenAsync: stream file + nama & tipe untuk response download. Stream ditutup oleh pemanggil
// (File(...) di controller/page menutupnya otomatis).
public record DocumentDownload(Stream Content, string FileName, string ContentType);

public interface IDocumentService
{
    // Kategori
    Task<List<DocumentCategoryResponseDto>> GetCategoriesAsync();
    Task<DocumentCategoryResponseDto> CreateCategoryAsync(DocumentCategoryCreateDto dto);
    Task<DocumentCategoryResponseDto> UpdateCategoryAsync(int id, DocumentCategoryCreateDto dto);
    Task DeleteCategoryAsync(int id);

    // Dokumen
    Task<List<DocumentResponseDto>> GetListAsync(UserContext actor, int? categoryId = null, int? ownerEmployeeId = null);
    Task<DocumentResponseDto> UploadAsync(
        UserContext actor, int categoryId, string title, int? ownerEmployeeId,
        string fileName, string? contentType, long length, Stream content);
    Task<DocumentResponseDto> UpdateAsync(int id, UserContext actor, DocumentUpdateDto dto);
    Task DeleteAsync(int id, UserContext actor);
    Task<DocumentDownload> OpenAsync(int id, UserContext actor);
}
