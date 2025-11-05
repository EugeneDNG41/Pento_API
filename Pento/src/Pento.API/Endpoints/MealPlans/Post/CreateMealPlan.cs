using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.MealPlans;

namespace Pento.API.Endpoints.MealPlans.Post;

internal sealed class CreateMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meal-plans", async (
            Request request,
            ICommandHandler<CreateMealPlanCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateMealPlanCommand(
                request.HouseholdId,
                request.RecipeId,
                request.Name,
                request.MealType,
                request.ScheduledDate,
                request.Servings,
                request.Notes,
                request.CreatedBy
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
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
