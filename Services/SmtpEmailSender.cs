using System.Net;
using System.Net.Mail;
using HRIS.Api.Common;
using Microsoft.Extensions.Options;

namespace HRIS.Api.Services;

// Email:Mode = Smtp. Memakai System.Net.Mail bawaan .NET (tanpa paket tambahan).
public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public SmtpEmailSender(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string toAddress, string? toName, string subject, string body, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
            throw new InvalidOperationException("Email:Host belum diisi padahal Email:Mode = Smtp.");

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Username))
            client.Credentials = new NetworkCredential(_options.Username, _options.Password);

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(string.IsNullOrWhiteSpace(toName) ? new MailAddress(toAddress) : new MailAddress(toAddress, toName));

        await client.SendMailAsync(message, cancellationToken);
    }
}
