using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IEmailOutboxRepository
{
    // Email Pending yang sudah waktunya dicoba (NextAttemptAt kosong atau sudah lewat).
    Task<List<EmailOutbox>> GetDueAsync(DateTime utcNow, int take);
    Task UpdateAsync(EmailOutbox email);
}
