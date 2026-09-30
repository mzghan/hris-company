namespace HRIS.Api.Services;

public interface IEmailSender
{
    // Melempar exception kalau gagal; EmailDispatchJob yang mengatur retry.
    Task SendAsync(string toAddress, string? toName, string subject, string body, CancellationToken cancellationToken);
}
