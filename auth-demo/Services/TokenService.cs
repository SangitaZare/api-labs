using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthDemo.Api.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthDemo.Api.Services;

public class TokenService(IConfiguration config, UserStore users)
{
    public TokenResponse IssueTokens(AppUser user)
    {
        var jwt = config.GetSection("Jwt");
        var accessExpires = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("AccessTokenMinutes"));

        // 1. Access token (JWT): short-lived, carries identity + role as claims
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Issuer = jwt["Issuer"],
            Audience = jwt["Audience"],
            Expires = accessExpires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                SecurityAlgorithms.HmacSha256)
        };
        var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);

        // 2. Refresh token: long-lived random string, stored server-side so it can be revoked
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(jwt.GetValue<int>("RefreshTokenDays"))
        };
        users.SaveRefreshToken(refreshToken);

        return new TokenResponse(accessToken, accessExpires, refreshToken.Token);
    }
}