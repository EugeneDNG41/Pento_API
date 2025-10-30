using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.API.Endpoints.Recipes.Put;

internal sealed class UpdateRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("recipes/{id:guid}",
            async (HttpContext context) =>
            {
                var id = Guid.Parse((string)context.Request.RouteValues["id"]!);
                Request? request = await context.Request.ReadFromJsonAsync<Request>();
                ICommandHandler<UpdateRecipeCommand> handler = context.RequestServices.GetRequiredService<ICommandHandler<UpdateRecipeCommand>>();
                CancellationToken cancellationToken = context.RequestAborted;

                var command = new UpdateRecipeCommand(
                    id,
                    request!.Title,
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

                Result result = await handler.Handle(command, cancellationToken);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem("Failed to update recipe.");
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
        public DifficultyLevel? DifficultyLevel { get; init; }
        public string? ImageUrl { get; init; }
        public Guid? CreatedBy { get; init; }
        public bool IsPublic { get; init; }
    }
}
