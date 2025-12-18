using System.ComponentModel.DataAnnotations;

namespace HealthCare.Models;

public class DoctorProfile
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; }

    // Approval flow
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAtUtc { get; set; }

    // License upload
    public string? LicenseFileName { get; set; }
    public string? LicenseFilePath { get; set; }
    public DateTime? LicenseUploadedAtUtc { get; set; }
}
