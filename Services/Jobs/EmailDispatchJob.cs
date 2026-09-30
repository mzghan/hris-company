using HRIS.Api.Common;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
using Microsoft.Extensions.Options;

namespace HRIS.Api.Services.Jobs;

// Mengirim email dari antrean TRX_Email_Outbox. Gagal kirim tidak pernah mengganggu proses bisnis:
// email dicoba lagi dengan jeda yang makin lama (2, 4, 8, ... menit) sampai Email:MaxAttempts,
// lalu ditandai Failed.
public class EmailDispatchJob : IRecurringJob
{
    private const int BatchSize = 50;

    private readonly EmailOptions _options;
    private readonly ILogger<EmailDispatchJob> _logger;

    public EmailDispatchJob(IOptions<EmailOptions> options, ILogger<EmailDispatchJob> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string Name => "EmailDispatch";

    public TimeSpan Interval => TimeSpan.FromSeconds(Math.Max(5, _options.PollSeconds));

    public async Task RunAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        // Mode Off: tidak ada email yang diantrekan, dan sisa antrean lama sengaja dibiarkan.
        if (string.Equals(_options.Mode, "Off", StringComparison.OrdinalIgnoreCase)) return;

        var repository = services.GetRequiredService<IEmailOutboxRepository>();
        var sender = services.GetRequiredService<IEmailSender>();

        var due = await repository.GetDueAsync(DateTime.UtcNow, BatchSize);
        foreach (var mail in due)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await sender.SendAsync(mail.ToAddress, mail.ToName, mail.Subject, mail.Body, cancellationToken);
                mail.Status = EmailStatus.Sent;
                mail.SentAt = DateTime.UtcNow;
                mail.LastError = null;
                mail.NextAttemptAt = null;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                mail.Attempts += 1;
                mail.LastError = ex.Message.Length <= 500 ? ex.Message : ex.Message[..500];

                if (mail.Attempts >= Math.Max(1, _options.MaxAttempts))
                {
                    mail.Status = EmailStatus.Failed;
                    _logger.LogError(ex, "Email #{Id} ke {To} gagal permanen setelah {Attempts} percobaan", mail.Id, mail.ToAddress, mail.Attempts);
                }
                else
                {
                    mail.NextAttemptAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, mail.Attempts));
                    _logger.LogWarning(ex, "Email #{Id} ke {To} gagal (percobaan {Attempts}), dicoba lagi nanti", mail.Id, mail.ToAddress, mail.Attempts);
                }
            }

            await repository.UpdateAsync(mail);
        }

        if (due.Count > 0)
            _logger.LogInformation("EmailDispatch memproses {Count} email", due.Count);
    }
}
