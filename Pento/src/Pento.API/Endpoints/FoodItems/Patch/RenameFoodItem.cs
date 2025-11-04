
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Rename;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class RenameFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/name", async (
            Guid foodItemId,
            string? name,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<RenameFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new RenameFoodItemCommand(foodItemId, name, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Renames a specific food item");
    }
}
