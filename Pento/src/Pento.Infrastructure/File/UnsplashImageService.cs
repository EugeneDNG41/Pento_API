using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pento.Application.Abstractions.File;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.File;
internal sealed class UnsplashImageService(HttpClient httpClient, IConfiguration config)
    : IUnsplashImageService
{
    public async Task<Result<Uri>> GetImageUrlAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            string? accessKey = config["Unsplash:AccessKey"];
            string url = $"https://api.unsplash.com/search/photos?query={Uri.EscapeDataString(query)}&per_page=1&client_id={accessKey}";
            HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using JsonDocument doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            string? img = doc.RootElement.GetProperty("results")[0].GetProperty("urls").GetProperty("regular").GetString();
            return Result.Success(new Uri(img!));
        }
        catch (Exception ex)
        {
            return Result.Failure<Uri>(
                Error.Problem("Unsplash.FetchFailed", $"Failed to fetch image: {ex.Message}"));
        }
    }
}
