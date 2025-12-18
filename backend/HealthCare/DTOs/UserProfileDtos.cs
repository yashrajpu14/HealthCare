using System.ComponentModel.DataAnnotations;

namespace HealthCare.DTOs;

public record UserProfileResponse(
    string Name,
    string Email,
    string? Phone,
    string Role
);

public class UpdateUserProfileRequest
{
    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }
}
