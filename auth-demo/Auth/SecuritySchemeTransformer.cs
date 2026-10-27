using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AuthDemo.Api.Auth;

// Declares Basic + Bearer in the OpenAPI document so Swagger UI shows an "Authorize" button
public class SecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste ONLY the accessToken (no 'Bearer ' prefix, no quotes)"
        };

        document.Components.SecuritySchemes["Basic"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            Description = "e.g. admin / Admin@123"
        };

        foreach (var path in document.Paths.Values)
        {
            if (path.Operations is null) continue;

            foreach (var operation in path.Operations.Values)
            {
                operation.Security ??= [];
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Basic", document)] = []
                });
            }
        }

        return Task.CompletedTask;
    }
}