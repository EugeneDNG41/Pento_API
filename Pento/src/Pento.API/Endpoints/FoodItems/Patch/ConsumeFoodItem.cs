
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.AdjustQuantity;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class ConsumeFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/consumption", async (
            Guid foodItemId,
            decimal quantity,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<AdjustFoodItemQuantityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new AdjustFoodItemQuantityCommand(foodItemId, quantity, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Consumes a specific quantity of a food item");
    }
}
