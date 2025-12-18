using global::HealthCare.DTOs;
using global::HealthCare.Repositories.Interfaces;
using global::HealthCare.Services.Interfaces;

namespace HealthCare.Services.Implementations;



public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    public UserService(IUserRepository users) => _users = users;

    public async Task<UserProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null) return null;

        return new UserProfileResponse(user.Name, user.Email, user.Phone, user.Role);
    }

    public async Task<(bool ok, string? error, UserProfileResponse? data)> UpdateMyProfileAsync(
        Guid userId,
        UpdateUserProfileRequest req,
        CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null) return (false, "UNAUTHORIZED", null);

        var name = (req.Name ?? "").Trim();
        var email = (req.Email ?? "").Trim().ToLowerInvariant();
        var phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone.Trim();

        if (string.IsNullOrWhiteSpace(name)) return (false, "Name is required", null);
        if (string.IsNullOrWhiteSpace(email)) return (false, "Email is required", null);

        var emailExists = await _users.EmailExistsAsync(email, userId, ct);
        if (emailExists) return (false, "Email already in use", null);

        user.Name = name;
        user.Email = email;
        user.Phone = phone;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _users.SaveChangesAsync(ct);

        return (true, null, new UserProfileResponse(user.Name, user.Email, user.Phone, user.Role));
    }
}
