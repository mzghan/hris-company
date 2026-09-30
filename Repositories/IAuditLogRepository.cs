using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<List<AuditLog>> GetRecentAsync(int take);
}
