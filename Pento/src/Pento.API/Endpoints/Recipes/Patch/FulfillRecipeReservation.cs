using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Patch;

internal sealed class FulfillRecipeReservation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("recipe-reservations/{id:guid}/fulfill", async (
       Guid id,
       FulfillRequest request,
       ICommandHandler<FulfillRecipeReservationCommand, Guid> handler,
       CancellationToken cancellationToken
   ) =>
        {
            var command = new FulfillRecipeReservationCommand(
                ReservationId: id,
                NewQuantity: request.Quantity,
                UnitId: request.UnitId
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
   .WithTags(Tags.Recipes)
   .RequireAuthorization();

    }

    internal sealed class FulfillRequest
    {
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
