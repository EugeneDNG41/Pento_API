using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Create.From_Recipe;
using Pento.Domain.MealPlans;

namespace Pento.API.Endpoints.MealPlans.Post;

internal sealed class CreateMealPlanFromRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meal-plans/from-recipe", async (
            Request dto,
            ICommandHandler<CreateMealPlanFromRecipeCommand, MealPlanAutoReserveResult> handler,
            CancellationToken ct
        ) =>
        {
            var cmd = new CreateMealPlanFromRecipeCommand(
                dto.RecipeId,
                dto.MealType,
                dto.ScheduledDate,
                dto.Servings
            );

            Domain.Abstractions.Result<MealPlanAutoReserveResult> result = await handler.Handle(cmd, ct);

            return result.Match(
                x => Results.Ok(x),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlans)          
        .RequireAuthorization();           
    }

    internal sealed class Request
    {
        public Guid RecipeId { get; init; }
        public MealType MealType { get; init; }
        public DateOnly ScheduledDate { get; init; }
        public int Servings { get; init; }
    }
}
