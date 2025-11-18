using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans.Post;

internal sealed class CreateMealPlanReservation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meal-plan-reservations", async (
            Request request,
            ICommandHandler<CreateMealPlanReservationCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CreateMealPlanReservationCommand(
                FoodItemId: request.FoodItemId,
                MealPlanId: request.MealPlanId,
                Quantity: request.Quantity,
                UnitId: request.UnitId
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlans)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public Guid FoodItemId { get; init; }
        public Guid MealPlanId { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
