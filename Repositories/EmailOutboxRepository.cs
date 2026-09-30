using HRIS.Api.Data;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class EmailOutboxRepository : IEmailOutboxRepository
{
    private readonly AppDbContext _context;

    public EmailOutboxRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmailOutbox>> GetDueAsync(DateTime utcNow, int take) =>
        await _context.EmailOutbox
            .Where(e => e.Status == EmailStatus.Pending
                        && (e.NextAttemptAt == null || e.NextAttemptAt <= utcNow))
            .OrderBy(e => e.Id)
            .Take(take)
            .ToListAsync();

    public async Task UpdateAsync(EmailOutbox email)
    {
        _context.EmailOutbox.Update(email);
        await _context.SaveChangesAsync();
    }
}
