using HealthCare.DTOs.Admin;
using HealthCare.Repositories.Interfaces;
using HealthCare.Services.Interfaces;
using HealthCare.Utils;
using HealthCare.Utils.Interfaces;
using System.Security.Cryptography;

namespace HealthCare.Services;

public sealed class AdminDoctorService : IAdminDoctorService
{
    private readonly IDoctorAdminRepository _repo;
    private readonly IEmailSender _email;

    public AdminDoctorService(IDoctorAdminRepository repo, IEmailSender email)
    {
        _repo = repo;
        _email = email;
    }

    public async Task<List<DoctorPendingResponse>> GetPendingDoctorsAsync(CancellationToken ct)
    {
        var pending = await _repo.GetPendingDoctorsAsync(ct);
        return pending.Select(p => new DoctorPendingResponse(
            p.UserId,
            p.User?.Name ?? "",
            p.User?.Email ?? "",
            p.User?.Phone,
            p.IsApproved,
            p.LicenseFileName,
            p.CreatedAtUtc
        )).ToList();
    }

    public async Task ApproveDoctorAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _repo.GetDoctorProfileByUserIdAsync(userId, ct);
        if (profile == null) throw new InvalidOperationException("Doctor profile not found.");

        if (profile.IsApproved) return;

        if (string.IsNullOrWhiteSpace(profile.LicenseFilePath))
            throw new InvalidOperationException("Doctor has not uploaded license.");

        var user = profile.User ?? await _repo.GetUserByIdAsync(userId, ct);
        if (user == null) throw new InvalidOperationException("User not found.");

        // Ensure role
        user.Role = "Doctor";

        // Generate initial password and set it (credentials email)
        var tempPassword = GenerateStrongPassword(12);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

        profile.IsApproved = true;
        profile.ApprovedAtUtc = DateTime.UtcNow;

        await _repo.SaveAsync(ct);

        // Email credentials (Graph)
        await _email.SendAsync(new EmailRequest
        {
            To = user.Email,
            ToName = user.Name,
            Subject = "Your Doctor Account is Approved",
            HtmlBody = $@"
                <p>Hi {System.Net.WebUtility.HtmlEncode(user.Name)},</p>
                <p>Your doctor account has been approved.</p>
                <p><b>Login email:</b> {System.Net.WebUtility.HtmlEncode(user.Email)}<br/>
                   <b>Temporary password:</b> {System.Net.WebUtility.HtmlEncode(tempPassword)}</p>
                <p>Please login and change your password immediately.</p>"
        }, ct);
    }

    private static string GenerateStrongPassword(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
        var rnd = RandomNumberGenerator.GetBytes(length);
        var result = new char[length];
        for (int i = 0; i < length; i++)
            result[i] = chars[rnd[i] % chars.Length];
        return new string(result);
    }
}
