using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log)
    {
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetRecentAsync(int take) =>
        await _context.AuditLogs
            .Include(l => l.User)
            .OrderByDescending(l => l.CreatedAt).ThenByDescending(l => l.Id)
            .Take(take)
            .ToListAsync();
}
