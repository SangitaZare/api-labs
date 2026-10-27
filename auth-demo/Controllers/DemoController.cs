using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public() => Ok("Anyone can see this.");

    [Authorize(AuthenticationSchemes = "Basic")]
    [HttpGet("basic")]
    public IActionResult BasicOnly() =>
        Ok($"Hello {User.Identity!.Name} (Basic auth). Role: {User.FindFirstValue(ClaimTypes.Role)}");

    [Authorize]   // uses the default scheme = JWT
    [HttpGet("jwt")]
    public IActionResult JwtOnly() =>
        Ok($"Hello {User.Identity!.Name} (JWT). Role: {User.FindFirstValue(ClaimTypes.Role)}");

    [Authorize(AuthenticationSchemes = $"Basic,{JwtBearerDefaults.AuthenticationScheme}")]
    [HttpGet("either")]
    public IActionResult Either() => Ok($"Hello {User.Identity!.Name} - Basic OR JWT both work here.");

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly() => Ok($"Welcome, Admin {User.Identity!.Name}!");
}