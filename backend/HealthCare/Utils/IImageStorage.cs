namespace HealthCare.Utils;

public interface IImageStorage
{
    Task<(string url, string publicId)> UploadUserImageAsync(IFormFile file, string userId, CancellationToken ct = default);
    Task DeleteAsync(string publicId, CancellationToken ct = default);
}
