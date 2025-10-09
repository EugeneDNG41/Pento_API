using MediatR;
using Pento.API.Extensions;
using Pento.Application.Recipes.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes;

internal sealed class CreateRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateRecipeCommand(
                request.Title,
                request.Description,
                request.PrepTimeMinutes,
                request.CookTimeMinutes,
                request.Notes,
                request.Servings,
                request.DifficultyLevel,
                request.CaloriesPerServing,
                request.ImageUrl is not null ? new Uri(request.ImageUrl) : null,
                request.CreatedBy,
                request.IsPublic
            );

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/recipes/{id}", id),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes);
    }

    internal sealed class Request
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int PrepTimeMinutes { get; init; }
        public int CookTimeMinutes { get; init; }
        public string? Notes { get; init; }
        public int? Servings { get; init; }
        public string? DifficultyLevel { get; init; }
        public int? CaloriesPerServing { get; init; }
        public string? ImageUrl { get; init; } 
        public Guid CreatedBy { get; init; }
        public bool IsPublic { get; init; }
    }
}
