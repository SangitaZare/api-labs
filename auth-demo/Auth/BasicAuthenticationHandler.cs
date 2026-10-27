using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using AuthDemo.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AuthDemo.Api.Auth;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    UserStore users)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? header = Request.Headers.Authorization;

        // No "Basic ..." header → not our job; let other schemes try
        if (!AuthenticationHeaderValue.TryParse(header, out var auth) ||
            !"Basic".Equals(auth.Scheme, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrEmpty(auth.Parameter))
            return Task.FromResult(AuthenticateResult.NoResult());

        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(auth.Parameter));
        }
        catch (FormatException)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Base64 in Authorization header"));
        }

        var separator = decoded.IndexOf(':');
        if (separator < 0)
            return Task.FromResult(AuthenticateResult.Fail("Invalid Basic credentials format"));

        var user = users.ValidateCredentials(decoded[..separator], decoded[(separator + 1)..]);
        if (user is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"AuthDemo\"";
        return base.HandleChallengeAsync(properties);
    }
}