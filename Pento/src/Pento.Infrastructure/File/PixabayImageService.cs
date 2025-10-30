﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pento.Application.Abstractions.File;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.File;
internal sealed class PixabayImageService(HttpClient httpClient, IConfiguration config)
    :IPixabayImageService
{
    public async Task<Result<Uri>> GetImageUrlAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            string? apiKey = config["Pixabay:ApiKey"];
            string url = $"https://pixabay.com/api/?key={apiKey}&q={Uri.EscapeDataString(query)}&image_type=photo&orientation=horizontal&per_page=3&safesearch=true&order=popular";

            HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using JsonDocument doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            JsonElement hits = doc.RootElement.GetProperty("hits");
            if (hits.GetArrayLength() == 0)
            {
                return Result.Failure<Uri>(Error.Problem("Pixabay.NoResults", $"No images found for '{query}'"));
            }

            string? imageUrl = hits[0].GetProperty("largeImageURL").GetString();
            return Result.Success(new Uri(imageUrl!));
        }
        catch (Exception ex)
        {
            return Result.Failure<Uri>(
                Error.Problem("Pixabay.FetchFailed", $"Failed to fetch image: {ex.Message}")
            );
        }
    }
}
