﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);
        string extension = Path.GetExtension(originalFileName);
        string safeName = Path.GetFileNameWithoutExtension(originalFileName).Replace(" ", "_");
        return $"{timestamp}_{Guid.NewGuid():N}_{safeName}{extension}";
    }

    public async Task<Result<string>> UploadFileAsync(IFormFile file, string fileTypeCategory = "general", CancellationToken cancellationToken = default)
    {
        if (!AllowedFileTypes.TryGetValue(fileTypeCategory, out string[]? allowedTypes))
        {
            return Result.Failure<string>(
                Error.Failure("Blob.InvalidCategory", $"Unknown category '{fileTypeCategory}'."));
        }

        if (!allowedTypes.Contains(file.ContentType))
        {
            return Result.Failure<string>(
                Error.Conflict("Blob.InvalidType", $"File type '{file.ContentType}' is not allowed for category '{fileTypeCategory}'."));
        }

        try
        {
            string fileName = GenerateFileName(file.FileName);
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(fileTypeCategory);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

            BlobClient blob = container.GetBlobClient(fileName);
            using Stream stream = file.OpenReadStream();

            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: cancellationToken);
            return Result.Success(blob.Uri.AbsoluteUri);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(
                Error.Problem("Blob.UploadFailed", $"Unexpected error during upload: {ex.Message}"));
        }
    }



    public Task<Result<string>> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default)
        => UploadFileAsync(file, "images", cancellationToken);

    public Task<Result<string>> UploadVideoAsync(IFormFile file, CancellationToken cancellationToken = default)
        => UploadFileAsync(file, "videos", cancellationToken);


    public async Task<bool> DeleteFileAsync(string fileName, string fileTypeCategory = "general", CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(fileTypeCategory);
            BlobClient blob = container.GetBlobClient(fileName);
            Azure.Response<bool> response = await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            return response.Value;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> DeleteImageAsync(string fileName, CancellationToken cancellationToken = default)
        => DeleteFileAsync(fileName, "images", cancellationToken);

    public Task<bool> DeleteVideoAsync(string fileName, CancellationToken cancellationToken = default)
        => DeleteFileAsync(fileName, "videos", cancellationToken);

}
