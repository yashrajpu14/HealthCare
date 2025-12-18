using HealthCare.Data;
using HealthCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCare.Controllers;

[ApiController]
[Route("api/doctor")]
[Authorize(Roles = "Doctor")]
public class DoctorController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DoctorController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost("license")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadLicense([FromForm] IFormFile license, CancellationToken ct)
    {
        if (license == null || license.Length == 0) return BadRequest("License file required.");

        var ext = Path.GetExtension(license.FileName).ToLowerInvariant();
        var allowed = new HashSet<string> { ".pdf", ".png", ".jpg", ".jpeg" };
        if (!allowed.Contains(ext)) return BadRequest("Only PDF/PNG/JPG allowed.");

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var profile = _db.DoctorProfiles.FirstOrDefault(x => x.UserId == userId);
        if (profile == null)
        {
            profile = new DoctorProfile { UserId = userId };
            _db.DoctorProfiles.Add(profile);
        }

        var uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads", "licenses");
        Directory.CreateDirectory(uploadsRoot);

        var safeName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
        var fullPath = Path.Combine(uploadsRoot, safeName);

        await using (var fs = new FileStream(fullPath, FileMode.Create))
            await license.CopyToAsync(fs, ct);

        profile.LicenseFileName = license.FileName;
        profile.LicenseFilePath = fullPath;
        profile.LicenseUploadedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "License uploaded successfully" });
    }
}
