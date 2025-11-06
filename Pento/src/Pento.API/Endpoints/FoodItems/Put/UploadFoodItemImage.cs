using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.UploadImage;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Put;

internal sealed class UploadFoodItemImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("food-items/{id:guid}/image", async (
            Guid id, 
            IFormFile? file,
            [FromIfMatchHeader] int version,
            ICommandHandler<UploadFoodItemImageCommand, Uri> handler, 
            CancellationToken ct) =>
            {
                Result<Uri> result = await handler.Handle(new UploadFoodItemImageCommand(id, file, version), ct);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithTags(Tags.FoodItems);
    }
}

