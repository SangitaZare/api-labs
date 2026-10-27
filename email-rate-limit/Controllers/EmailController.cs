using EmailRateLimitApi.Models;
using EmailRateLimitApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EmailRateLimitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly PdfService _pdfService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailService emailService, PdfService pdfService,
                           ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _pdfService = pdfService;
        _logger = logger;
    }

    [HttpPost("send")]
    [EnableRateLimiting("EmailPolicy")]   // rate limit applies only to this endpoint
    public async Task<IActionResult> Send(SendEmailRequest request)
    {
        // 1. Create the PDF in memory
        var pdfBytes = _pdfService.CreateReport(request.Name);

        // 2. Write the email text
        var body = $"<h3>Hello {request.Name},</h3>" +
                   "<p>Please find your employee report attached as a PDF.</p>" +
                   "<p>Regards,<br/>Day 22 Demo API</p>";

        try
        {
            // 3. Send the email with the PDF attached
            await _emailService.SendAsync(request.ToEmail, request.Name,
                "Your Employee Report", body, "EmployeeReport.pdf", pdfBytes);

            return Ok(new { message = $"Email with PDF sent to {request.ToEmail}." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sending failed");
            // Returning the reason helps while learning. Hide it in production.
            return StatusCode(500, new { message = "Email could not be sent.", detail = ex.Message });
        }
    }
}