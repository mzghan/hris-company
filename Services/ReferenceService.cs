using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class ReferenceService : IReferenceService
{
    private readonly IReferenceRepository _repository;

    public ReferenceService(IReferenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ReferenceItemDto>> GetOptionsAsync(string type)
    {
        var rows = await _repository.GetOptionsAsync(type)
            ?? throw new NotFoundException($"Tabel referensi '{type}' tidak dikenal.");

        return rows.Select(r => new ReferenceItemDto { Id = r.Id, Name = r.Name }).ToList();
    }
}
