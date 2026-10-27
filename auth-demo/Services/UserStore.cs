using AuthDemo.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Api.Services;

// In-memory store so Day 16 stays focused on auth. Replace with EF Core + SQL Server later.
public class UserStore
{
    private readonly List<AppUser> _users = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly PasswordHasher<AppUser> _hasher = new();
    private readonly object _lock = new();

    public UserStore()
    {
        Register("admin", "Admin@123", "Admin");
        Register("user1", "User@123", "User");
    }

    public AppUser? Register(string username, string password, string role)
    {
        lock (_lock)
        {
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return null;

            var user = new AppUser { Id = _users.Count + 1, Username = username, Role = role };
            user.PasswordHash = _hasher.HashPassword(user, password);
            _users.Add(user);
            return user;
        }
    }

    public AppUser? ValidateCredentials(string username, string password)
    {
        var user = FindByUsername(username);
        if (user is null) return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }

    public AppUser? FindByUsername(string username)
    {
        lock (_lock)
            return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public AppUser? FindById(int id)
    {
        lock (_lock) return _users.FirstOrDefault(u => u.Id == id);
    }

    public bool SetRole(string username, string role)
    {
        var user = FindByUsername(username);
        if (user is null) return false;
        user.Role = role;
        return true;
    }

    public void SaveRefreshToken(RefreshToken token)
    {
        lock (_lock) _refreshTokens.Add(token);
    }

    public RefreshToken? FindRefreshToken(string token)
    {
        lock (_lock) return _refreshTokens.FirstOrDefault(t => t.Token == token);
    }

    public void RevokeAllForUser(int userId)
    {
        lock (_lock)
            foreach (var t in _refreshTokens.Where(t => t.UserId == userId && t.IsActive))
                t.RevokedAt = DateTime.UtcNow;
    }
}