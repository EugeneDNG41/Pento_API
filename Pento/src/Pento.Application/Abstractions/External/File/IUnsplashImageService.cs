using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.External.File;

public interface IUnsplashImageService
{
    Task<Result<Uri>> GetImageUrlAsync(string query, CancellationToken cancellationToken = default);
}
