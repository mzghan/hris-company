using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentCategory>> GetAllCategoriesAsync() =>
        await _context.DocumentCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<DocumentCategory?> GetCategoryByIdAsync(int id) =>
        await _context.DocumentCategories
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Dictionary<int, int>> GetDocumentCountsByCategoryAsync() =>
        await _context.Documents
            .Where(d => d.IsActive)
            .GroupBy(d => d.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

    public async Task<bool> CategoryHasDocumentsAsync(int categoryId) =>
        await _context.Documents.AnyAsync(d => d.CategoryId == categoryId);

    public async Task<DocumentCategory> AddCategoryAsync(DocumentCategory category)
    {
        _context.DocumentCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateCategoryAsync(DocumentCategory category)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(DocumentCategory category)
    {
        _context.DocumentCategories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Document>> GetListAsync(int? categoryId, int? ownerEmployeeId, int? viewerEmployeeId, bool includeAllPersonal)
    {
        var query = _context.Documents
            .Include(d => d.Category).ThenInclude(c => c.Parent)
            .Include(d => d.OwnerEmployee)
            .Where(d => d.IsActive);

        // Karyawan biasa: dokumen umum + dokumen pribadi miliknya sendiri. HR/Support: semuanya.
        if (!includeAllPersonal)
        {
            var viewerId = viewerEmployeeId ?? -1;
            query = query.Where(d => d.OwnerEmployeeId == null || d.OwnerEmployeeId == viewerId);
        }

        if (categoryId is not null)
            query = query.Where(d => d.CategoryId == categoryId.Value);

        if (ownerEmployeeId is not null)
            query = query.Where(d => d.OwnerEmployeeId == ownerEmployeeId.Value);

        return await query.OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.Id).ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(int id) =>
        await _context.Documents
            .Include(d => d.Category).ThenInclude(c => c.Parent)
            .Include(d => d.OwnerEmployee)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Document> AddAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task UpdateAsync(Document document)
    {
        await _context.SaveChangesAsync();
    }
}
