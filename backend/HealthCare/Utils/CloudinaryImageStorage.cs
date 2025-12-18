using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace HealthCare.Utils;

public sealed class CloudinaryImageStorage : IImageStorage
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _opt;

    public CloudinaryImageStorage(IOptions<CloudinaryOptions> opt)
    {
        _opt = opt.Value;

        var account = new Account(_opt.CloudName, _opt.ApiKey, _opt.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<(string url, string publicId)> UploadUserImageAsync(IFormFile file, string userId, CancellationToken ct = default)
    {
        if (file.Length <= 0) throw new ArgumentException("Empty file.");
        if (!file.ContentType.StartsWith("image/")) throw new ArgumentException("Only image files are allowed.");

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = _opt.Folder,
            PublicId = $"user_{userId}_{Guid.NewGuid():N}",
            Overwrite = false,
            Transformation = new Transformation()
                .Width(512).Height(512).Crop("fill").Gravity("face")
                .Quality("auto").FetchFormat("auto")
        };

        var res = await _cloudinary.UploadAsync(uploadParams, ct);
        if (res.StatusCode != System.Net.HttpStatusCode.OK && res.StatusCode != System.Net.HttpStatusCode.Created)
            throw new InvalidOperationException($"Cloudinary upload failed: {res.Error?.Message}");

        return (res.SecureUrl.ToString(), res.PublicId);
    }

    public async Task DeleteAsync(string publicId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(publicId)) return;

        var delParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
        await _cloudinary.DestroyAsync(delParams);
    }
}
