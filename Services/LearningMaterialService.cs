using HRIS.Api.Common;
using HRIS.Api.DTOs.Learning;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class LearningMaterialService : ILearningMaterialService
{
    private readonly ILearningMaterialRepository _repository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IAuditLogService _auditLogService;

    public LearningMaterialService(ILearningMaterialRepository repository, IDocumentRepository documentRepository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _documentRepository = documentRepository;
        _auditLogService = auditLogService;
    }

    public async Task<List<LearningMaterialResponseDto>> GetAllAsync() =>
        (await _repository.GetAllAsync()).Select(ToDto).ToList();

    public async Task<LearningMaterialResponseDto> CreateAsync(LearningMaterialCreateDto dto, UserContext actor)
    {
        EnsureManage(actor);
        var document = await GetLearningDocumentAsync(dto.DocumentId);
        var material = await _repository.AddAsync(new LearningMaterial
        {
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DocumentId = document.Id
        });
        return ToDto((await _repository.GetByIdAsync(material.Id))!);
    }

    public async Task<LearningMaterialResponseDto> UpdateAsync(int id, LearningMaterialCreateDto dto, UserContext actor)
    {
        EnsureManage(actor);
        var material = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Materi pembelajaran tidak ditemukan.");
        _ = await GetLearningDocumentAsync(dto.DocumentId);
        material.Title = dto.Title.Trim();
        material.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        material.DocumentId = dto.DocumentId;
        await _repository.UpdateAsync(material);
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "LearningMaterial.Update", nameof(LearningMaterial), id, material.Title);
        return ToDto((await _repository.GetByIdAsync(id))!);
    }

    public async Task DeleteAsync(int id, UserContext actor)
    {
        EnsureManage(actor);
        var material = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Materi pembelajaran tidak ditemukan.");
        material.IsActive = false;
        await _repository.UpdateAsync(material);
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "LearningMaterial.Delete", nameof(LearningMaterial), id, material.Title);
    }

    private async Task<Document> GetLearningDocumentAsync(int id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null || !document.IsActive) throw new BadRequestException("Dokumen pembelajaran tidak ditemukan.");
        var category = document.Category;
        if (category is null || !string.Equals(category.Name, "Learning", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Dokumen materi harus berada di kategori Learning.");
        return document;
    }

    private static void EnsureManage(UserContext actor)
    {
        if (!actor.IsHrOrSupport) throw new ForbiddenException("Hanya HR/Support yang boleh mengelola materi pembelajaran.");
    }

    private static LearningMaterialResponseDto ToDto(LearningMaterial x) => new()
    {
        Id = x.Id, Title = x.Title, Description = x.Description, DocumentId = x.DocumentId,
        FileName = x.Document?.FileName ?? string.Empty, ContentType = x.Document?.ContentType,
        SizeBytes = x.Document?.SizeBytes ?? 0, CreatedAt = x.CreatedAt
    };
}
