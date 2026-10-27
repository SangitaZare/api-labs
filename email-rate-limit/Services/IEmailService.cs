namespace EmailRateLimitApi.Services;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject,
                   string htmlBody, string attachmentName, byte[] attachmentBytes);
}