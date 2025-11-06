using System.Globalization;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.File;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.File;
public sealed class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    private static readonly Dictionary<string, string[]> AllowedFileTypes = new()
    {
        { "general", new[] { "image/jpeg", "image/png", "application/pdf", "text/plain", "video/mp4" } },
        { "images", new[] { "image/jpeg", "image/png", "image/webp", "image/gif" } },
        { "videos", new[] { "video/mp4", "video/mpeg", "video/quicktime" } },
        { "gemini", new[] { "image/jpeg", "image/png", "application/pdf", "text/plain", "audio/mpeg", "audio/wav" } }
    };

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    private static string GenerateFileName(string originalFileName)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        string extension = Path.GetExtension(originalFileName);
        string safeName = Path.GetFileNameWithoutExtension(originalFileName).Replace(" ", "_");
        return $"{timestamp}_{Guid.NewGuid():N}_{safeName}{extension}";
    }

    public async Task<Result<Uri>> UploadFileAsync(IFormFile file, string domain, string fileTypeCategory = "general", CancellationToken cancellationToken = default)
    {
        if (!AllowedFileTypes.TryGetValue(fileTypeCategory, out string[]? allowedTypes))
        {
            return Result.Failure<Uri>(
                Error.Problem("Blob.InvalidCategory", $"Unknown category '{fileTypeCategory}'."));
        }

        if (!allowedTypes.Contains(file.ContentType))
        {
            return Result.Failure<Uri>(
                Error.Problem("Blob.InvalidType", $"File type '{file.ContentType}' is not allowed for category '{fileTypeCategory}'."));
        }

        try
        {
            string fileName = GenerateFileName(file.FileName);
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient($"{domain.ToLower(CultureInfo.InvariantCulture)}-{fileTypeCategory}");
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

            BlobClient blob = container.GetBlobClient(fileName);

            using Stream stream = file.OpenReadStream();

            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: cancellationToken);
            return Result.Success(blob.Uri);
        }
        catch (Exception ex)
        {
            return Result.Failure<Uri>(
                Error.Failure("Blob.UploadFailed", $"Unexpected error during upload: {ex.Message}"));
        }
    }



    public Task<Result<Uri>> UploadImageAsync(IFormFile file, string domain, CancellationToken cancellationToken = default)
        => UploadFileAsync(file, domain, "images", cancellationToken);

    public Task<Result<Uri>> UploadVideoAsync(IFormFile file, string domain, CancellationToken cancellationToken = default)
        => UploadFileAsync(file, domain, "videos", cancellationToken);



    public async Task<bool> DeleteFileAsync(string domain, string filePath, string fileTypeCategory = "general", CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient($"{domain.ToLower(CultureInfo.InvariantCulture)}-{fileTypeCategory}");
            BlobClient blob = container.GetBlobClient(filePath);
            Azure.Response<bool> response = await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            return response.Value;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> DeleteImageAsync(string domain, string fileName, CancellationToken cancellationToken = default)
        => DeleteFileAsync(domain, fileName, "images", cancellationToken);

    public Task<bool> DeleteVideoAsync(string domain, string fileName, CancellationToken cancellationToken = default)
        => DeleteFileAsync(domain, fileName, "videos", cancellationToken);


}
