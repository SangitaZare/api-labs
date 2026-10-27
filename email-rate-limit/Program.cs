using System.Threading.RateLimiting;
using EmailRateLimitApi.Models;
using EmailRateLimitApi.Services;
using Microsoft.AspNetCore.RateLimiting;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Free license for learning projects (QuestPDF Community)
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Email: read the "EmailSettings" section and register the services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<PdfService>();

// Rate limiting: 3 requests per minute, counted separately for each IP address
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("EmailPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,                    // allowed requests
                Window = TimeSpan.FromMinutes(1),   // per this time window
                QueueLimit = 0                      // do not queue extra requests
            }));

    options.OnRejected = async (context, token) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();
        }

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Too many requests. Please try again after some time."
        }, token);
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRateLimiter();          // must come before MapControllers
app.UseAuthorization();
app.MapControllers();

app.Run();