using System.ComponentModel.DataAnnotations;

namespace HealthCare.DTOs.Admin;

public sealed class ApproveDoctorRequest
{
    [Required]
    public Guid UserId { get; set; }
}
