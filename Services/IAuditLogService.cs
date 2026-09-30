using HRIS.Api.DTOs.Audit;

namespace HRIS.Api.Services;

public interface IAuditLogService
{
    Task LogAsync(int? userId, string action, string? entityType = null, int? entityId = null, string? detail = null);
    Task<List<AuditLogResponseDto>> GetRecentAsync(int take = 100);
}
