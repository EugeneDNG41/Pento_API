using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Reserve.Fullfill;
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
   .WithTags(Tags.MealPlans)
   .RequireAuthorization();

        app.MapPatch("meal-plans/{mealPlanId:guid}/recipes/{recipeId:guid}/fulfill", async (
            Guid mealPlanId,
            Guid recipeId,
            ICommandHandler<FulfillMealPlanRecipeCommand, Guid> handler,
            CancellationToken ct
        ) =>
                {
                    var cmd = new FulfillMealPlanRecipeCommand(mealPlanId, recipeId);

                    Result<Guid> result = await handler.Handle(cmd, ct);

                    return result.Match(
                        Results.Ok,
                        CustomResults.Problem
                    );
                })
        .WithTags(Tags.MealPlans)
        .RequireAuthorization();
        app.MapPatch("meal-plans/{mealPlanId:guid}/fulfill", async (
                Guid mealPlanId,
                ICommandHandler<FulfillMealPlanCommand, Guid> handler,
                CancellationToken ct
            ) =>
                    {
                        var cmd = new FulfillMealPlanCommand(mealPlanId);
                        Result<Guid> result = await handler.Handle(cmd, ct);

                        return result.Match(
                            Results.Ok,
                            CustomResults.Problem
                        );
                    })
            .WithTags(Tags.MealPlans)
            .RequireAuthorization();

    }

    internal sealed class FulfillRequest
    {
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
