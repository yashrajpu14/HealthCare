using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthCheckController : Controller
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });

}
