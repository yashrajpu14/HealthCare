using HealthCare.DTOs.Admin;
using HealthCare.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Controllers;

[ApiController]
[Route("api/admin/doctors")]
[Authorize(Roles = "Admin")]
public class AdminDoctorController : ControllerBase
{
    private readonly IAdminDoctorService _svc;

    public AdminDoctorController(IAdminDoctorService svc)
    {
        _svc = svc;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<List<DoctorPendingResponse>>> Pending(CancellationToken ct)
        => Ok(await _svc.GetPendingDoctorsAsync(ct));

    [HttpPost("approve")]
    public async Task<IActionResult> Approve([FromBody] ApproveDoctorRequest req, CancellationToken ct)
    {
        await _svc.ApproveDoctorAsync(req.UserId, ct);
        return Ok(new { message = "Doctor approved and credentials emailed." });
    }
}
