using HealthCare.Models;
namespace HealthCare.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, Guid excludeUserId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
