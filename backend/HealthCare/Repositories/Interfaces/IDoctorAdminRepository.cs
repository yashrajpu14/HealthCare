using HealthCare.Models;

namespace HealthCare.Repositories.Interfaces;

public interface IDoctorAdminRepository
{
    Task<List<DoctorProfile>> GetPendingDoctorsAsync(CancellationToken ct);
    Task<DoctorProfile?> GetDoctorProfileByUserIdAsync(Guid userId, CancellationToken ct);

    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
