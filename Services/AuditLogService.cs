using HRIS.Api.DTOs.Audit;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(int? userId, string action, string? entityType = null, int? entityId = null, string? detail = null)
    {
        await _repository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Detail = detail is { Length: > 500 } ? detail[..500] : detail
        });
    }

    public async Task<List<AuditLogResponseDto>> GetRecentAsync(int take = 100)
    {
        var logs = await _repository.GetRecentAsync(Math.Clamp(take, 1, 500));
        return logs.Select(l => new AuditLogResponseDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Username = l.User?.Username,
            Action = l.Action,
            EntityType = l.EntityType,
            EntityId = l.EntityId,
            Detail = l.Detail,
            CreatedAt = l.CreatedAt
        }).ToList();
    }
}
