using Microsoft.EntityFrameworkCore;

namespace HealthCare.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Example table (you can rename)
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}
