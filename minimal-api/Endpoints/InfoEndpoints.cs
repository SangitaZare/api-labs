
using MinimalApiDemo.Settings;
using Microsoft.Extensions.Options;

namespace MinimalApiDemo.Endpoints;

public static class InfoEndpoints
{
    public static void MapInfoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/info", (
            IOptions<AppSettings> options,
            IWebHostEnvironment environment) =>
        {
            var settings = options.Value;

            return Results.Ok(new
            {
                appName = settings.AppName,
                environment = environment.EnvironmentName,
                defaultPageSize = settings.DefaultPageSize,
                maxPageSize = settings.MaxPageSize,
                apiKeyConfigured = !string.IsNullOrWhiteSpace(settings.ApiKey)
            });
        });
    }
}
