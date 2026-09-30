using HRIS.Api.Common;
using HRIS.Api.DTOs.Document;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
using Microsoft.Extensions.Options;

namespace HRIS.Api.Services;

public class DocumentService : IDocumentService
{
    // Dipakai kalau Documents:AllowedExtensions kosong. Sengaja tanpa .html/.svg/.js/.exe.
    private static readonly string[] DefaultExtensions =
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv", ".png", ".jpg", ".jpeg"
    };

    private readonly IDocumentRepository _repository;
    private readonly IReferenceRepository _referenceRepository;
    private readonly IFileStorage _storage;
    private readonly IAuditLogService _auditLogService;
    private readonly DocumentOptions _options;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        IDocumentRepository repository,
        IReferenceRepository referenceRepository,
        IFileStorage storage,
        IAuditLogService auditLogService,
        IOptions<DocumentOptions> options,
        ILogger<DocumentService> logger)
    {
        _repository = repository;
        _referenceRepository = referenceRepository;
        _storage = storage;
        _auditLogService = auditLogService;
        _options = options.Value;
        _logger = logger;
    }

    // ================= Kategori =================

    public async Task<List<DocumentCategoryResponseDto>> GetCategoriesAsync()
    {
        var categories = await _repository.GetAllCategoriesAsync();
        var counts = await _repository.GetDocumentCountsByCategoryAsync();
        var byId = categories.ToDictionary(c => c.Id);

        return categories
            .Select(c => new DocumentCategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId,
                FullName = BuildFullName(c, byId),
                DocumentCount = counts.TryGetValue(c.Id, out var n) ? n : 0
            })
            .OrderBy(c => c.FullName)
            .ToList();
    }

    public async Task<DocumentCategoryResponseDto> CreateCategoryAsync(DocumentCategoryCreateDto dto)
    {
        if (dto.ParentId is not null && await _repository.GetCategoryByIdAsync(dto.ParentId.Value) is null)
            throw new BadRequestException($"ParentId {dto.ParentId} tidak ditemukan.");

        var category = await _repository.AddCategoryAsync(new DocumentCategory
        {
            Name = dto.Name.Trim(),
            ParentId = dto.ParentId
        });

        return await GetCategoryDtoAsync(category.Id);
    }

    public async Task<DocumentCategoryResponseDto> UpdateCategoryAsync(int id, DocumentCategoryCreateDto dto)
    {
        var category = await _repository.GetCategoryByIdAsync(id)
            ?? throw new NotFoundException($"Kategori dengan id {id} tidak ditemukan.");

        if (dto.ParentId is not null)
        {
            if (dto.ParentId == id)
                throw new BadRequestException("Kategori tidak bisa menjadi induk untuk dirinya sendiri.");

            // Cegah siklus: telusuri ke atas dari induk baru, tidak boleh bertemu kategori ini.
            var all = (await _repository.GetAllCategoriesAsync()).ToDictionary(c => c.Id);
            var cursor = dto.ParentId;
            while (cursor is not null && all.TryGetValue(cursor.Value, out var parent))
            {
                if (parent.Id == id)
                    throw new BadRequestException("Induk yang dipilih berada di bawah kategori ini, akan membuat siklus.");
                cursor = parent.ParentId;
            }

            if (!all.ContainsKey(dto.ParentId.Value))
                throw new BadRequestException($"ParentId {dto.ParentId} tidak ditemukan.");
        }

        category.Name = dto.Name.Trim();
        category.ParentId = dto.ParentId;
        await _repository.UpdateCategoryAsync(category);

        return await GetCategoryDtoAsync(id);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _repository.GetCategoryByIdAsync(id)
            ?? throw new NotFoundException($"Kategori dengan id {id} tidak ditemukan.");

        if (category.Children.Any())
            throw new BadRequestException("Kategori masih punya sub-kategori, hapus atau pindahkan dulu.");

        if (await _repository.CategoryHasDocumentsAsync(id))
            throw new BadRequestException("Kategori masih dipakai dokumen (termasuk yang sudah dihapus), tidak bisa dihapus.");

        await _repository.DeleteCategoryAsync(category);
    }

    // ================= Dokumen =================

    public async Task<List<DocumentResponseDto>> GetListAsync(UserContext actor, int? categoryId = null, int? ownerEmployeeId = null)
    {
        var documents = await _repository.GetListAsync(categoryId, ownerEmployeeId, actor.EmployeeId, actor.IsHrOrSupport);
        return documents.Select(ToDto).ToList();
    }

    public async Task<DocumentResponseDto> UploadAsync(
        UserContext actor, int categoryId, string title, int? ownerEmployeeId,
        string fileName, string? contentType, long length, Stream content)
    {
        if (!actor.IsHrOrSupport)
            throw new ForbiddenException("Hanya HR/Support yang boleh mengunggah dokumen.");

        title = title.Trim();
        if (title.Length == 0)
            throw new BadRequestException("Judul dokumen wajib diisi.");

        // Path.GetFileName membuang komponen folder dari nama file yang dikirim client.
        var safeName = Path.GetFileName(fileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(safeName))
            throw new BadRequestException("Nama file tidak valid.");

        var extension = Path.GetExtension(safeName).ToLowerInvariant();
        var allowed = _options.AllowedExtensions.Length > 0 ? _options.AllowedExtensions : DefaultExtensions;
        if (!allowed.Contains(extension, StringComparer.OrdinalIgnoreCase))
            throw new BadRequestException($"Tipe file '{extension}' tidak diizinkan. Yang diizinkan: {string.Join(", ", allowed)}.");

        if (length <= 0)
            throw new BadRequestException("File kosong.");

        var maxBytes = (long)Math.Max(1, _options.MaxSizeMb) * 1024 * 1024;
        if (length > maxBytes)
            throw new BadRequestException($"Ukuran file melebihi batas {_options.MaxSizeMb} MB.");

        var category = await _repository.GetCategoryByIdAsync(categoryId);
        if (category is null || !category.IsActive)
            throw new BadRequestException($"CategoryId {categoryId} tidak ditemukan.");

        if (ownerEmployeeId is not null && !await _referenceRepository.ExistsAsync<Employee>(ownerEmployeeId.Value))
            throw new BadRequestException($"OwnerEmployeeId {ownerEmployeeId} tidak ditemukan.");

        var storedPath = await _storage.SaveAsync(content, extension);
        try
        {
            var document = await _repository.AddAsync(new Document
            {
                CategoryId = categoryId,
                Title = Truncate(title, 200),
                FileName = Truncate(safeName, 255),
                StoredPath = storedPath,
                ContentType = contentType is null ? null : Truncate(contentType, 150),
                SizeBytes = length,
                OwnerEmployeeId = ownerEmployeeId,
                UploadedByUserId = actor.UserId
            });

            if (actor.IsSupport)
                await _auditLogService.LogAsync(actor.UserId, "Document.Upload", nameof(Document), document.Id, Truncate(document.Title, 200));

            _logger.LogInformation("Dokumen {Id} '{Title}' diunggah oleh user {UserId}", document.Id, document.Title, actor.UserId);

            var created = await _repository.GetByIdAsync(document.Id);
            return ToDto(created!);
        }
        catch
        {
            // Jangan tinggalkan file yatim di disk kalau simpan metadata gagal.
            _storage.Delete(storedPath);
            throw;
        }
    }

    public async Task<DocumentResponseDto> UpdateAsync(int id, UserContext actor, DocumentUpdateDto dto)
    {
        if (!actor.IsHrOrSupport)
            throw new ForbiddenException("Hanya HR/Support yang boleh mengubah dokumen.");

        var document = await GetActiveOrThrowAsync(id);

        var category = await _repository.GetCategoryByIdAsync(dto.CategoryId);
        if (category is null || !category.IsActive)
            throw new BadRequestException($"CategoryId {dto.CategoryId} tidak ditemukan.");

        document.Title = Truncate(dto.Title.Trim(), 200);
        document.CategoryId = dto.CategoryId;
        await _repository.UpdateAsync(document);

        if (actor.IsSupport)
            await _auditLogService.LogAsync(actor.UserId, "Document.Update", nameof(Document), document.Id, document.Title);

        var updated = await _repository.GetByIdAsync(id);
        return ToDto(updated!);
    }

    public async Task DeleteAsync(int id, UserContext actor)
    {
        if (!actor.IsHrOrSupport)
            throw new ForbiddenException("Hanya HR/Support yang boleh menghapus dokumen.");

        var document = await GetActiveOrThrowAsync(id);

        // Soft delete: baris dan file dipertahankan (jejak audit), hanya disembunyikan.
        document.IsActive = false;
        await _repository.UpdateAsync(document);

        if (actor.IsSupport)
            await _auditLogService.LogAsync(actor.UserId, "Document.Delete", nameof(Document), document.Id, document.Title);
    }

    public async Task<DocumentDownload> OpenAsync(int id, UserContext actor)
    {
        var document = await GetActiveOrThrowAsync(id);

        var isOwner = actor.EmployeeId is not null && actor.EmployeeId == document.OwnerEmployeeId;
        if (document.OwnerEmployeeId is not null && !isOwner && !actor.IsHrOrSupport)
            throw new ForbiddenException("Kamu tidak punya akses ke dokumen ini.");

        // Support melewati pengecekan kepemilikan, jadi pembukaan dokumen pribadi orang lain dicatat.
        if (actor.IsSupport && document.OwnerEmployeeId is not null && !isOwner)
            await _auditLogService.LogAsync(actor.UserId, "Document.SupportDownload", nameof(Document), document.Id, document.Title);

        Stream stream;
        try
        {
            stream = _storage.OpenRead(document.StoredPath);
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundException("File dokumen tidak ditemukan di penyimpanan. Hubungi Support.");
        }

        return new DocumentDownload(stream, document.FileName, document.ContentType ?? "application/octet-stream");
    }

    // ================= Helper =================

    private async Task<Document> GetActiveOrThrowAsync(int id)
    {
        var document = await _repository.GetByIdAsync(id);
        if (document is null || !document.IsActive)
            throw new NotFoundException($"Dokumen dengan id {id} tidak ditemukan.");
        return document;
    }

    private async Task<DocumentCategoryResponseDto> GetCategoryDtoAsync(int id)
    {
        var all = await GetCategoriesAsync();
        return all.First(c => c.Id == id);
    }

    private static string BuildFullName(DocumentCategory category, Dictionary<int, DocumentCategory> byId)
    {
        var parts = new List<string> { category.Name };
        var cursor = category.ParentId;
        var guard = 0;
        while (cursor is not null && byId.TryGetValue(cursor.Value, out var parent) && guard++ < 20)
        {
            parts.Insert(0, parent.Name);
            cursor = parent.ParentId;
        }
        return string.Join(" > ", parts);
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max];

    private static DocumentResponseDto ToDto(Document d) => new()
    {
        Id = d.Id,
        CategoryId = d.CategoryId,
        CategoryName = d.Category?.Parent is null
            ? d.Category?.Name ?? string.Empty
            : $"{d.Category.Parent.Name} > {d.Category.Name}",
        Title = d.Title,
        FileName = d.FileName,
        ContentType = d.ContentType,
        SizeBytes = d.SizeBytes,
        OwnerEmployeeId = d.OwnerEmployeeId,
        OwnerEmployeeName = d.OwnerEmployee?.FullName,
        CreatedAt = d.CreatedAt
    };
}
