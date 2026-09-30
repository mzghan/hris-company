using HRIS.Api.DTOs.Reference;

namespace HRIS.Api.Services;

public interface IReferenceService
{
    Task<List<ReferenceItemDto>> GetOptionsAsync(string type);
}
