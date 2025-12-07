using System.Globalization;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.External.File;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.External.File;

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
        catch
        {
            return Result.Failure<Uri>(BlobServiceErrors.UploadFailed);
        }
    }



    public Task<Result<Uri>> UploadImageAsync(IFormFile file, string domain, CancellationToken cancellationToken = default)
        => UploadFileAsync(file, domain, "images", cancellationToken);

    public Task<Result<Uri>> UploadVideoAsync(IFormFile file, string domain, CancellationToken cancellationToken = default)
        => UploadFileAsync(file, domain, "videos", cancellationToken);



    public async Task<Result> DeleteFileAsync(string domain, string filePath, string fileTypeCategory = "general", CancellationToken cancellationToken = default)
    {
        try
        {
            if (!filePath.Contains(".blob.core.windows.net"))
            {
                return Result.Success();
            }
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient($"{domain.ToLower(CultureInfo.InvariantCulture)}-{fileTypeCategory}");
            BlobClient blob = container.GetBlobClient(filePath);
            await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            return Result.Success();
        }
        catch
        {
            return Result.Failure(BlobServiceErrors.DeleteFailed);
        }
    }

    public Task<Result> DeleteImageAsync(string domain, string fileName, CancellationToken cancellationToken = default)
        => DeleteFileAsync(domain, fileName, "images", cancellationToken);

    public Task<Result> DeleteVideoAsync(string domain, string fileName, CancellationToken cancellationToken = default)
        => DeleteFileAsync(domain, fileName, "videos", cancellationToken);

    public async Task<Result<Uri>> UploadImageFromUrlAsync(
        string source,
        string domain,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var http = new HttpClient();

            HttpResponseMessage response = await http.GetAsync(source, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<Uri>(
                    Error.Problem(
                        "Blob.DownloadFailed",
                        $"Failed to download image from URL: {source}"
                    )
                );
            }

            string contentType = response.Content.Headers.ContentType?.MediaType ?? "";

            if (!AllowedFileTypes["images"].Contains(contentType))
            {
                return Result.Failure<Uri>(
                    Error.Problem(
                        "Blob.InvalidType",
                        $"Downloaded content type '{contentType}' is not allowed for images."
                    )
                );
            }

            string extension = contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => Path.GetExtension(source)
            };

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            string fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}{Guid.NewGuid():N}{extension}";

            BlobContainerClient container =
                _blobServiceClient.GetBlobContainerClient($"{domain.ToLowerInvariant()}images");

            await container.CreateIfNotExistsAsync(
                publicAccessType: PublicAccessType.Blob,
                cancellationToken: cancellationToken);

            BlobClient blob = container.GetBlobClient(fileName);

            using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            await blob.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken
            );

            return Result.Success(blob.Uri);
        }
        catch
        {
            return Result.Failure<Uri>(BlobServiceErrors.UploadFailed);
        }
    }


}
public static class BlobServiceErrors
{
    public static Error UploadFailed
        => Error.Failure("Blob.UploadFailed", "Failed to upload file to blob storage");
    public static Error DeleteFailed
        => Error.Failure("Blob.DeleteFailed", "Failed to delete file from blob storage");
}
