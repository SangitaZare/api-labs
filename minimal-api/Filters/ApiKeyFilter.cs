using Microsoft.Extensions.Options;
using MinimalApiDemo.Settings;

namespace MinimalApiDemo.Filters;

public class ApiKeyFilter : IEndpointFilter
{
    private readonly AppSettings _settings;

    public ApiKeyFilter(IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        // Check if the X-Api-Key header is present
        if (!httpContext.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
        {
            return Results.Json(new { message = "API Key is missing." }, statusCode: 401);
        }

        // Validate the API key against configuration settings
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) || _settings.ApiKey != extractedApiKey)
        {
            return Results.Json(new { message = "Invalid API Key." }, statusCode: 401);
        }

        // If valid, proceed to the endpoint handler
        return await next(context);
    }
}