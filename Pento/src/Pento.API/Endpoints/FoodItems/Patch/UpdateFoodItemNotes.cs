
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.UpdateNotes;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class UpdateFoodItemNotes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/notes", async (
            Guid foodItemId,
            string notes,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<UpdateFoodItemNotesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateFoodItemNotesCommand(foodItemId, notes, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Updates the notes of a specific food item");
    }
}
