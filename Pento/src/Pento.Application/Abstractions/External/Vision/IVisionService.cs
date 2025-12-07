using Azure.AI.Vision.ImageAnalysis;

namespace Pento.Application.Abstractions.External.Vision;

public interface IVisionService
{
    Task<ImageAnalysisResult> DetectObjectsAsync(Uri imageUrl);
    Task<ImageAnalysisResult> AnalyzeTextAsync(Uri imageUrl);

}
