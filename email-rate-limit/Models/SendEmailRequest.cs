using System.ComponentModel.DataAnnotations;

namespace EmailRateLimitApi.Models;

public class SendEmailRequest
{
    [Required]
    [EmailAddress]
    public string ToEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}