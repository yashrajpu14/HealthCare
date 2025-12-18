using HealthCare.DTOs;
using HealthCare.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCare.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    private Guid GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idStr is null || !Guid.TryParse(idStr, out var userId))
            throw new UnauthorizedAccessException("Invalid or missing user id in token");
        return userId;
    }

    // GET: /api/users/me
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile(CancellationToken ct)
    {
        var userId = GetUserId();
        var profile = await _users.GetMyProfileAsync(userId, ct);

        if (profile == null) return Unauthorized();
        return Ok(profile);
    }

    // PUT: /api/users/me
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponse>> UpdateMyProfile(
    [FromForm] UpdateUserProfileRequest req,
    CancellationToken ct)
    {
        var userId = GetUserId();

        var (ok, error, data) = await _users.UpdateMyProfileAsync(userId, req, ct);

        if (!ok && error == "UNAUTHORIZED") return Unauthorized();
        if (!ok && error == "Email already in use") return Conflict(error);
        if (!ok) return BadRequest(error);

        return Ok(data);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var userId = GetUserId();

        var (ok, error) = await _users.ChangeMyPasswordAsync(userId, req, ct);

        if (!ok && error == "UNAUTHORIZED") return Unauthorized();
        if (!ok) return BadRequest(error);

        return Ok(new { message = "Password changed successfully" });
    }
}
