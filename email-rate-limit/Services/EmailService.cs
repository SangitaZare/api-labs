using EmailRateLimitApi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailRateLimitApi.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(string toEmail, string toName, string subject,
                                string htmlBody, string attachmentName, byte[] attachmentBytes)
    {
        // 1. Build the message
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        // 2. Body + PDF attachment
        var builder = new BodyBuilder { HtmlBody = htmlBody };
        builder.Attachments.Add(attachmentName, attachmentBytes, new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        // 3. Connect, log in, send
        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.Username, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}