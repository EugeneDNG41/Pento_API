using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Reserve.Cancel;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans.Patch;

internal sealed class CancelMealPlanReservation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("mealplan-reservations/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelMealPlanReservationCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CancelMealPlanReservationCommand(id);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Reservations)
        .RequireAuthorization();
        app.MapPatch("meal-plans/{mealPlanId:guid}/recipes/{recipeId:guid}/cancel", async (
          Guid mealPlanId,
          Guid recipeId,
          ICommandHandler<CancelMealPlanRecipeCommand, Guid> handler,
          CancellationToken ct
      ) =>
        {
            var cmd = new CancelMealPlanRecipeCommand(mealPlanId, recipeId);

            Result<Guid> result = await handler.Handle(cmd, ct);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
      .WithTags(Tags.MealPlans)
      .RequireAuthorization();
    }
}
