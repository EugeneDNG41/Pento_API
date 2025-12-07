using Pento.Application.Abstractions.External.Vision;

namespace Pento.API.Endpoints.Visions.Post;

internal sealed class AnalyzeVisionByUrl : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("vision/analyze-url", async (
            Request request,
            IVisionService visionService,
            CancellationToken cancellationToken) =>
        {
            if (!Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out Uri? uri))
            {
                return Results.BadRequest("Invalid image URL.");
            }

            Azure.AI.Vision.ImageAnalysis.ImageAnalysisResult result = await visionService.DetectObjectsAsync(uri);

            VisionAnalysisResponse response = VisionMapper.ToResponse(result);

            return Results.Ok(response);
        })
        .WithTags("Vision");
    }

    internal sealed class Request
    {
        public string ImageUrl { get; init; } = default!;
    }
}
