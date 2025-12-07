using Azure.AI.Vision.ImageAnalysis;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.Vision;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;

namespace Pento.API.Endpoints.Visions.Post;

internal sealed class AnalyzeOcrEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("vision/ocr", async (
            Request request,
            IVisionService visionService,
            CancellationToken cancellationToken) =>
        {

            
            if (!Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out Uri? uri))
            {
                return Results.BadRequest("Invalid image URL.");
            }

            ImageAnalysisResult result = await visionService.AnalyzeTextAsync(uri);

            List<OcrLine> lines = result.Read?.Blocks
                .SelectMany(b => b.Lines)
                .Select(l => new OcrLine
                {
                    Text = l.Text,

                    BoundingBox = l.BoundingPolygon
                        .SelectMany(p => new[] { p.X, p.Y })
                        .ToArray()
                })
                .ToList() ?? new List<OcrLine>();

            var response = new OcrResponse
            {
                Caption = result.Caption?.Text,
                Lines = lines
            };

            return Results.Ok(response);
        })
        .WithTags("Vision").RequireAuthorization();
    }

    internal sealed class Request
    {
        public string ImageUrl { get; init; } = default!;
    }

    internal sealed class OcrResponse
    {
        public string? Caption { get; set; }
        public List<OcrLine> Lines { get; set; } = new();
    }

    internal sealed class OcrLine
    {
        public string Text { get; set; } = default!;
        public int[] BoundingBox { get; set; } = Array.Empty<int>();
    }
}
