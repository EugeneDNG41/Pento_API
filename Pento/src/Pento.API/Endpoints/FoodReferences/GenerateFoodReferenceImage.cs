    using MediatR;
    using Pento.API.Endpoints;
    using Pento.API.Extensions;
    using Pento.Application.FoodReferences.GenerateImage;
    using Pento.Domain.Abstractions;

    namespace Pento.API.Endpoints.FoodReferences;

    internal sealed class GenerateFoodReferenceImage : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("food-references/{id:guid}/generate-image",
                async (Guid id, ISender sender, CancellationToken ct) =>
                {
                    var cmd = new GenerateFoodReferenceImageCommand(id);
                    Result<string> result = await sender.Send(cmd, ct);
                    return result.Match(
                        guid => Results.Ok(new { Id = guid }),
                        error => CustomResults.Problem(error)
                    );
                })
                .DisableAntiforgery()
                .WithTags(Tags.FoodReferences);
        }
    }
