using System.Security.Claims;
using AuthDemo.Api.Models;
using AuthDemo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserStore users, TokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        // New users always start as "User"; only an Admin can promote them
        var user = users.Register(request.Username, request.Password, "User");
        return user is null
            ? Conflict("Username already exists")
            : Ok(new { user.Id, user.Username, user.Role });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = users.ValidateCredentials(request.Username, request.Password);
        return user is null
            ? Unauthorized("Invalid username or password")
            : Ok(tokens.IssueTokens(user));
    }

    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshRequest request)
    {
        var stored = users.FindRefreshToken(request.RefreshToken);
        if (stored is null) return Unauthorized("Unknown refresh token");

        // A token that was already rotated is being used again → likely stolen.
        // Revoke every session for this user.
        if (stored.ReplacedByToken is not null)
        {
            users.RevokeAllForUser(stored.UserId);
            return Unauthorized("Refresh token reuse detected - all sessions revoked");
        }

        if (!stored.IsActive) return Unauthorized("Refresh token expired or revoked");

        var user = users.FindById(stored.UserId);
        if (user is null) return Unauthorized();

        var response = tokens.IssueTokens(user);   // reads the user's CURRENT role
        stored.RevokedAt = DateTime.UtcNow;        // rotation: old refresh token is now dead
        stored.ReplacedByToken = response.RefreshToken;
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout(RefreshRequest request)
    {
        var stored = users.FindRefreshToken(request.RefreshToken);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (stored is not null && stored.UserId.ToString() == userId)
            stored.RevokedAt ??= DateTime.UtcNow;

        return NoContent();
    }
}