using global::HealthCare.DTOs;
using global::HealthCare.Repositories.Interfaces;
using global::HealthCare.Services.Interfaces;
using HealthCare.Utils;

namespace HealthCare.Services.Implementations;



public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IImageStorage _images;
    public UserService(IUserRepository users, IImageStorage images)
    {
        _users = users;
        _images = images;
    }
    public async Task<UserProfileResponse?> GetMyProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null) return null;

        return new UserProfileResponse(user.Name, user.Email, user.Phone, user.Role, user.ProfileImageUrl);
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

        if (await _users.EmailExistsAsync(email, userId, ct))
            return (false, "Email already in use", null);

        // NEW: upload image if provided
        if (req.ProfileImage is not null)
        {
            var oldPublicId = user.ProfileImagePublicId;

            var (url, publicId) = await _images.UploadUserImageAsync(req.ProfileImage, userId.ToString(), ct);

            user.ProfileImageUrl = url;
            user.ProfileImagePublicId = publicId;

            // delete old after successful upload (best-effort)
            if (!string.IsNullOrWhiteSpace(oldPublicId))
            {
                try { await _images.DeleteAsync(oldPublicId, ct); } catch { /* ignore */ }
            }
        }

        user.Name = name;
        user.Email = email;
        user.Phone = phone;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _users.SaveChangesAsync(ct);

        return (true, null, new UserProfileResponse(
            user.Name, user.Email, user.Phone, user.Role,
            user.ProfileImageUrl
        ));
    }

    public async Task<(bool ok, string error)> ChangeMyPasswordAsync(
        Guid userId,
        ChangePasswordRequest req,
        CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user == null) return (false, "UNAUTHORIZED");

        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash))
            return (false, "Current password is incorrect");

        if (BCrypt.Net.BCrypt.Verify(req.NewPassword, user.PasswordHash))
            return (false, "New password must be different from current password");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _users.SaveChangesAsync(ct);
        return (true, "");
    }


}
