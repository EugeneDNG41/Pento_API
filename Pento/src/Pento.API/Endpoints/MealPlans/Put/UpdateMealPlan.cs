using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.API.Endpoints.MealPlans.Put;

internal sealed class UpdateMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("meal-plans/{mealPlanId:guid}", async (
            Guid mealPlanId,
            Request request,
            ICommandHandler<UpdateMealPlanCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateMealPlanCommand(
                mealPlanId,
                request.MealType,
                request.ScheduledDate,
                request.Servings,
                request.Notes
            );

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.MealPlans);
    }

    internal sealed class Request
    {
        public MealType MealType { get; init; }
        public DateOnly ScheduledDate { get; init; }
        public int Servings { get; init; }
        public string? Notes { get; init; }
    }
}
