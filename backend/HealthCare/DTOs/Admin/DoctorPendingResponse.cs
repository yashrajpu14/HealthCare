namespace HealthCare.DTOs.Admin;

public sealed record DoctorPendingResponse(
    Guid UserId,
    string Name,
    string Email,
    string? Phone,
    bool IsApproved,
    string? LicenseFileName,
    DateTime CreatedAtUtc
);
