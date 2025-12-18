using HealthCare.DTOs;

namespace HealthCare.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken ct = default);
    Task<(bool ok, string? error, UserProfileResponse? data)> UpdateMyProfileAsync(Guid userId, UpdateUserProfileRequest req, CancellationToken ct = default);
    Task<(bool ok, string error)> ChangeMyPasswordAsync(
        Guid userId,
        ChangePasswordRequest req,
        CancellationToken ct);
}
