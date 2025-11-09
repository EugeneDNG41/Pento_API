using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.API.Endpoints.Recipes.Post;

internal sealed class CreateRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes", async (Request request, ICommandHandler<CreateRecipeCommand, Guid> handler, CancellationToken cancellationToken) =>
        {
            var command = new CreateRecipeCommand(
                request.Title,
                request.Description,
                request.PrepTimeMinutes,
                request.CookTimeMinutes,
                request.Notes,
                request.Servings,
                request.DifficultyLevel,
                request.ImageUrl is not null ? new Uri(request.ImageUrl) : null,
                request.CreatedBy,
                request.IsPublic
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

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
        public string Description { get; init; }
        public int PrepTimeMinutes { get; init; }
        public int CookTimeMinutes { get; init; }
        public string? Notes { get; init; }
        public int? Servings { get; init; }
        public DifficultyLevel? DifficultyLevel { get; init; }
        public string? ImageUrl { get; init; } 
        public Guid? CreatedBy { get; init; }
        public bool IsPublic { get; init; }
    }
}
