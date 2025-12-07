using Microsoft.AspNetCore.Http;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.External.File;

public interface IBlobService
{

    Task<Result<Uri>> UploadFileAsync(
        IFormFile file,
        string domain,
        string fileTypeCategory = "general",
        CancellationToken cancellationToken = default);


    Task<Result<Uri>> UploadImageAsync(
        IFormFile file,
        string domain,
        CancellationToken cancellationToken = default);


    Task<Result<Uri>> UploadVideoAsync(
        IFormFile file,
        string domain,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFileAsync(
        string domain,
        string filePath,
        string fileTypeCategory = "general",
        CancellationToken cancellationToken = default);


    Task<Result> DeleteImageAsync(
        string domain,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteVideoAsync(
        string domain,
        string fileName,
        CancellationToken cancellationToken = default);
    Task<Result<Uri>> UploadImageFromUrlAsync(
    string source,
    string domain,
    CancellationToken cancellationToken = default);
}
