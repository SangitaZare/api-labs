using AuthDemo.Api.Models;
using AuthDemo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]   // applies to every action in this controller
public class AdminController(UserStore users) : ControllerBase
{
    [HttpPut("users/{username}/role")]
    public IActionResult AssignRole(string username, AssignRoleRequest request)
    {
        if (request.Role is not ("Admin" or "User"))
            return BadRequest("Role must be 'Admin' or 'User'");

        return users.SetRole(username, request.Role)
            ? Ok($"{username} is now {request.Role}. They get the new role on their next login/refresh.")
            : NotFound($"User '{username}' not found");
    }
}