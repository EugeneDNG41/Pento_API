using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.API.Endpoints.Recipes.Post;

internal sealed class CreateDetailedRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes/detailed", async (
            Request request,
            ICommandHandler<CreateDetailedRecipeCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateDetailedRecipeCommand(
                request.Title,
                request.Description,
                request.PrepTimeMinutes,
                request.CookTimeMinutes,
                request.Notes,
                request.Servings,
                request.DifficultyLevel,
                request.Image,
                request.IsPublic,
                request.Ingredients?.Select(i =>
                    new RecipeIngredientRequest(
                        i.FoodRefId,
                        i.Quantity,
                        i.UnitId,
                        i.Notes
                    )).ToList() ?? new List<RecipeIngredientRequest>(),
                request.Directions?.Select(d =>
                    new RecipeDirectionRequest(
                        d.StepNumber,
                        d.Description,
                        d.Image
                    )).ToList() ?? new List<RecipeDirectionRequest>()
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { RecipeId = id }),
                CustomResults.Problem);
        })
        .WithTags(Tags.Recipes)
        .DisableAntiforgery()
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public int PrepTimeMinutes { get; init; }
        public int CookTimeMinutes { get; init; }
        public string? Notes { get; init; }
        public int? Servings { get; init; }
        public DifficultyLevel? DifficultyLevel { get; init; }
        public string? Image { get; init; }
        public bool IsPublic { get; init; }
        public List<Ingredient>? Ingredients { get; init; }
        public List<Direction>? Directions { get; init; }

        internal sealed class Ingredient
        {
            public Guid FoodRefId { get; init; }
            public decimal Quantity { get; init; }
            public Guid UnitId { get; init; }
            public string? Notes { get; init; }
        }

        internal sealed class Direction
        {
            public int StepNumber { get; init; }
            public string Description { get; init; }
            public string? Image { get; init; }
        }
    }
}
