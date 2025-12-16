using HealthCare.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Controllers;

[ApiController]
[Route("api/v1/todos")]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _db;
    public TodoController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _db.TodoItems.AsNoTracking().ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoItem item)
    {
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }
}
