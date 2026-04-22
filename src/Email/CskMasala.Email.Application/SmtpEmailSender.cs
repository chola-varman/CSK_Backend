using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CskMasala.Email.Application;

public class SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
{
    public async Task SendAsync(EmailMessage message, CancellationToken ct)
    {
        var host = config["SMTP_HOST"] ?? "smtp.gmail.com";
        var port = int.Parse(config["SMTP_PORT"] ?? "587");
        var user = config["SMTP_USER"] ?? string.Empty;
        var pass = config["SMTP_PASS"] ?? string.Empty;
        var from = config["SMTP_FROM"] ?? "noreply@cskmasala.com";

        if (string.IsNullOrWhiteSpace(user))
        {
            logger.LogWarning("SMTP credentials not configured. Skipping email to {To}", message.To);
            return;
        }

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress("CSK Masala", from));
        mime.To.Add(new MailboxAddress(string.Empty, message.To));
        mime.Subject = message.Subject;
        mime.Body = new TextPart("html") { Text = message.HtmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(user, pass, ct);
        await client.SendAsync(mime, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Email sent to {To}: {Subject}", message.To, message.Subject);
    }
}
