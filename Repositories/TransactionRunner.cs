using HRIS.Api.Data;

namespace HRIS.Api.Repositories;

public class TransactionRunner : ITransactionRunner
{
    private readonly AppDbContext _context;

    public TransactionRunner(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T> RunAsync<T>(Func<Task<T>> action)
    {
        if (_context.Database.CurrentTransaction is not null)
            return await action();

        // Kalau action melempar exception, transaction di-dispose tanpa commit (rollback otomatis).
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var result = await action();
        await transaction.CommitAsync();
        return result;
    }

    public async Task RunAsync(Func<Task> action)
    {
        await RunAsync<bool>(async () =>
        {
            await action();
            return true;
        });
    }
}
