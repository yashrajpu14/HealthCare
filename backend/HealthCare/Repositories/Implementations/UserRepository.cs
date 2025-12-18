using global::HealthCare.Data;
using global::HealthCare.Repositories.Interfaces;
using HealthCare.Models;
using Microsoft.EntityFrameworkCore;
namespace HealthCare.Repositories.Implementations;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, Guid excludeUserId, CancellationToken ct = default) =>
        _db.Users.AnyAsync(u => u.Email == email && u.Id != excludeUserId, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
