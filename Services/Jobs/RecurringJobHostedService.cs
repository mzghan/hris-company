namespace HRIS.Api.Services.Jobs;

// Satu hosted service yang menjalankan semua IRecurringJob. Error di satu job dicatat ke log
// dan tidak mengganggu job lain maupun eksekusi berikutnya.
// Catatan: jadwal disimpan di memori, jadi hanya cocok untuk satu instance aplikasi.
public class RecurringJobHostedService : BackgroundService
{
    private static readonly TimeSpan Tick = TimeSpan.FromSeconds(5);

    private readonly IEnumerable<IRecurringJob> _jobs;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecurringJobHostedService> _logger;

    public RecurringJobHostedService(
        IEnumerable<IRecurringJob> jobs,
        IServiceScopeFactory scopeFactory,
        ILogger<RecurringJobHostedService> logger)
    {
        _jobs = jobs;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var nextRun = _jobs.ToDictionary(j => j, _ => DateTime.UtcNow.Add(Tick));

        _logger.LogInformation("Background job aktif: {Jobs}", string.Join(", ", _jobs.Select(j => j.Name)));

        using var timer = new PeriodicTimer(Tick);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                foreach (var job in _jobs)
                {
                    if (DateTime.UtcNow < nextRun[job]) continue;

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        await job.RunAsync(scope.ServiceProvider, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Background job {Job} gagal", job.Name);
                    }

                    nextRun[job] = DateTime.UtcNow.Add(job.Interval);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Aplikasi berhenti, keluar dengan tenang.
        }
    }
}
