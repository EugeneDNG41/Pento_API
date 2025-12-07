using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans.Delete;

internal sealed class DeleteMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("meal-plans/{mealPlanId:guid}", async (
            Guid mealPlanId,
            ICommandHandler<DeleteMealPlanCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new DeleteMealPlanCommand(mealPlanId),
                cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.MealPlans);
    }
}
