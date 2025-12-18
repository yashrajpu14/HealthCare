using HealthCare.Data;
using HealthCare.Models;
using HealthCare.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories.Implementations;

public sealed class DoctorAdminRepository : IDoctorAdminRepository
{
    private readonly AppDbContext _db;
    public DoctorAdminRepository(AppDbContext db) => _db = db;

    public Task<List<DoctorProfile>> GetPendingDoctorsAsync(CancellationToken ct) =>
        _db.DoctorProfiles
           .Include(dp => dp.User)
           .Where(dp => !dp.IsApproved)
           .OrderBy(dp => dp.CreatedAtUtc)
           .ToListAsync(ct);

    public Task<DoctorProfile?> GetDoctorProfileByUserIdAsync(Guid userId, CancellationToken ct) =>
        _db.DoctorProfiles.Include(dp => dp.User)
           .FirstOrDefaultAsync(dp => dp.UserId == userId, ct);

    public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
