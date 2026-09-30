using HRIS.Api.Common;
using HRIS.Api.DTOs.Regulation;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class RegulationService : IRegulationService
{
    private readonly IRegulationRepository _repository;
    private readonly IAuditLogService _auditLogService;
    public RegulationService(IRegulationRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository; _auditLogService = auditLogService;
    }

    public async Task<List<RegulationResponseDto>> GetAllAsync(bool includeInactive, UserContext actor)
    {
        if (includeInactive && !actor.IsHrOrSupport) throw new ForbiddenException("Hanya HR/Support yang boleh melihat peraturan nonaktif.");
        return (await _repository.GetAllAsync(includeInactive)).Select(ToDto).ToList();
    }

    public async Task<RegulationResponseDto> CreateAsync(RegulationCreateDto dto, UserContext actor)
    {
        EnsureManage(actor);
        var regulation = await _repository.AddAsync(new Regulation { Title = dto.Title.Trim(), Content = dto.Content.Trim(), SortOrder = dto.SortOrder });
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "Regulation.Create", nameof(Regulation), regulation.Id, regulation.Title);
        return ToDto(regulation);
    }

    public async Task<RegulationResponseDto> UpdateAsync(int id, RegulationCreateDto dto, UserContext actor)
    {
        EnsureManage(actor);
        var regulation = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Peraturan tidak ditemukan.");
        regulation.Title = dto.Title.Trim(); regulation.Content = dto.Content.Trim(); regulation.SortOrder = dto.SortOrder;
        await _repository.UpdateAsync(regulation);
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "Regulation.Update", nameof(Regulation), id, regulation.Title);
        return ToDto(regulation);
    }

    public async Task DeleteAsync(int id, UserContext actor)
    {
        EnsureManage(actor);
        var regulation = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Peraturan tidak ditemukan.");
        regulation.IsActive = false;
        await _repository.UpdateAsync(regulation);
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "Regulation.Delete", nameof(Regulation), id, regulation.Title);
    }

    private static void EnsureManage(UserContext actor)
    {
        if (!actor.IsHrOrSupport) throw new ForbiddenException("Hanya HR/Support yang boleh mengelola peraturan.");
    }

    private static RegulationResponseDto ToDto(Regulation x) => new() { Id = x.Id, Title = x.Title, Content = x.Content, SortOrder = x.SortOrder, CreatedAt = x.CreatedAt };
}
