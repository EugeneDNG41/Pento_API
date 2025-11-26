using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

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
                request.FoodItemId,
                request.Quantity,
                request.UnitId,
                request.MealType,
                request.ScheduledDate,
                request.Servings
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Reservations)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public Guid FoodItemId { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }

        public MealType MealType { get; init; }
        public DateOnly ScheduledDate { get; init; }
        public int? Servings { get; init; }
    }
}
