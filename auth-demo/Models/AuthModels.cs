namespace AuthDemo.Api.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User";
}

public class RefreshToken
{
    public string Token { get; set; } = "";
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record AssignRoleRequest(string Role);
public record TokenResponse(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken);