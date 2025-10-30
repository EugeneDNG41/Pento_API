using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.API.Endpoints.MealPlans;

internal sealed class CreateMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meal-plans",
            async (Request req, ICommandHandler<CreateMealPlanCommand, Guid> handler, CancellationToken ct) =>
            {
                var cmd = new CreateMealPlanCommand(
                    req.HouseholdId,
                    req.RecipeId,
                    req.Name,
                    req.MealType,
                    req.ScheduledDate,
                    req.Servings,
                    req.Notes,
                    req.CreatedBy
                );

                Result<Guid> result = await handler.Handle(cmd, ct);

                return result.Match(
                    id => Results.Created($"/meal-plans/{id}", new { Id = id }),
                    CustomResults.Problem
                );
            })
            .WithTags(Tags.MealPlans);
    }

    internal sealed class Request
    {
        public Guid HouseholdId { get; init; }
        public Guid RecipeId { get; init; }
        public string Name { get; init; } = string.Empty;
        public MealType MealType { get; init; }
        public DateOnly ScheduledDate { get; init; }
        public int Servings { get; init; }
        public string? Notes { get; init; }
        public Guid CreatedBy { get; init; }
    }
}
