using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IDocumentRepository
{
    // Kategori
    Task<List<DocumentCategory>> GetAllCategoriesAsync();
    Task<DocumentCategory?> GetCategoryByIdAsync(int id);
    Task<Dictionary<int, int>> GetDocumentCountsByCategoryAsync();
    Task<bool> CategoryHasDocumentsAsync(int categoryId);
    Task<DocumentCategory> AddCategoryAsync(DocumentCategory category);
    Task UpdateCategoryAsync(DocumentCategory category);
    Task DeleteCategoryAsync(DocumentCategory category);

    // Dokumen. viewerEmployeeId / includeAllPersonal menentukan dokumen pribadi mana yang terlihat.
    Task<List<Document>> GetListAsync(int? categoryId, int? ownerEmployeeId, int? viewerEmployeeId, bool includeAllPersonal);
    Task<Document?> GetByIdAsync(int id);
    Task<Document> AddAsync(Document document);
    Task UpdateAsync(Document document);
}
