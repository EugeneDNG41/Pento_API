
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.UpdateExpirationDate;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class UpdateFoodItemExpirationDate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/expiration-date", async (
            Guid foodItemId,
            DateTime newExpirationDate,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<UpdateFoodItemExpirationDateCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateFoodItemExpirationDateCommand(foodItemId, newExpirationDate, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Updates the expiration date of a specific food item");
    }
}
