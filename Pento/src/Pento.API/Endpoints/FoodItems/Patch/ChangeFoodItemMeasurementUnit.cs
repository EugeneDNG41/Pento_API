
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.ChangeUnit;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class ChangeFoodItemMeasurementUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/measurement-unit", async (
            Guid foodItemId,
            Guid measurementUnitId,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<ChangeFoodItemMeasurementUnitCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ChangeFoodItemMeasurementUnitCommand(foodItemId, measurementUnitId, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Changes the measurement unit of a specific food item");
    }
}
