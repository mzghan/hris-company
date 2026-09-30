namespace HRIS.Api.Services;

// Untuk development (Email:Mode = Log): email tidak dikirim sungguhan, hanya ditulis ke log.
public class LogEmailSender : IEmailSender
{
    private readonly ILogger<LogEmailSender> _logger;

    public LogEmailSender(ILogger<LogEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toAddress, string? toName, string subject, string body, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[EMAIL-LOG] To: {To} | Subject: {Subject}", toAddress, subject);
        return Task.CompletedTask;
    }
}
