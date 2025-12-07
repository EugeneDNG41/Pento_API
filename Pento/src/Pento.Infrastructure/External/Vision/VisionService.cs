using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using Pento.Application.Abstractions.External.Vision;

namespace Pento.Infrastructure.External.Vision;

public class VisionService : IVisionService
{
    private readonly ImageAnalysisClient _client;

    public VisionService(IConfiguration config)
    {
        string? endpoint = config["Vision:Endpoint"];
        string? apiKey = config["Vision:ApiKey"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("Missing Vision:Endpoint in configuration.");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Missing Vision:ApiKey in configuration.");
        }

        _client = new ImageAnalysisClient(
           new Uri(endpoint),
           new AzureKeyCredential(apiKey)
       );
    }

    public async Task<ImageAnalysisResult> DetectObjectsAsync(Uri imageUrl)
    {
        ArgumentNullException.ThrowIfNull(imageUrl);

        VisualFeatures features =
            VisualFeatures.Objects |
            VisualFeatures.Caption |
            VisualFeatures.Tags;

        var options = new ImageAnalysisOptions
        {
            GenderNeutralCaption = true
        };

        ImageAnalysisResult result = await _client.AnalyzeAsync(
            imageUrl,
            features,
            options
        );

        return result;
    }
    public async Task<ImageAnalysisResult> AnalyzeTextAsync(Uri imageUrl)
    {
        ArgumentNullException.ThrowIfNull(imageUrl);

        VisualFeatures features =
            VisualFeatures.Read |
            VisualFeatures.Caption;

        var options = new ImageAnalysisOptions
        {
            GenderNeutralCaption = true
        };

        return await _client.AnalyzeAsync(imageUrl, features, options);
    }
}
