
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Discard;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class DiscardFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/discard", async (
            Guid foodItemId,
            decimal quantity,
            DiscardReason reason,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<DiscardFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {

            Result result = await handler.Handle(new DiscardFoodItemCommand(foodItemId, quantity, reason, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Discards a specific quantity of a food item for a given reason");
    }
}
