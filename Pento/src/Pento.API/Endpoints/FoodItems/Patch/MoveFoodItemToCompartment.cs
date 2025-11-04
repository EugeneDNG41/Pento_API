
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.MoveToCompartment;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class MoveFoodItemToCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/compartment", async (
            Guid foodItemId,
            Guid compartmentId,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<MoveFoodItemToCompartmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new MoveFoodItemToCompartmentCommand(foodItemId, compartmentId, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Moves a specific food item to a different compartment");
    }
}
