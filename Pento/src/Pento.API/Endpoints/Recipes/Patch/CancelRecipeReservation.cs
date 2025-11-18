using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Patch;

internal sealed class CancelRecipeReservation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("recipe-reservations/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelRecipeReservationCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CancelRecipeReservationCommand(id);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes)
        .RequireAuthorization();
    }
}
