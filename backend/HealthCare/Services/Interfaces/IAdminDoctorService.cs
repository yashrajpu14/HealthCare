using HealthCare.DTOs.Admin;

namespace HealthCare.Services.Interfaces;

public interface IAdminDoctorService
{
    Task<List<DoctorPendingResponse>> GetPendingDoctorsAsync(CancellationToken ct);
    Task ApproveDoctorAsync(Guid userId, CancellationToken ct);
}
