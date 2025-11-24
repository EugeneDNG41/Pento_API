using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans.Patch;
    
internal sealed class FullfillMealPlanReservation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("mealplan-reservations/{id:guid}/fulfill", async (
       Guid id,
       FulfillRequest request,
       ICommandHandler<FulfillMealPlanReservationCommand, Guid> handler,
       CancellationToken cancellationToken
   ) =>
        {
            var command = new FulfillMealPlanReservationCommand(
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
   .WithTags(Tags.Reservations)
   .RequireAuthorization();

    }

    internal sealed class FulfillRequest
    {
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
