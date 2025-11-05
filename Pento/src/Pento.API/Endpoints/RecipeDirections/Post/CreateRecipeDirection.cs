using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeDirections.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.RecipeDirections.Post;

internal sealed class CreateRecipeDirection : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipe-directions", async (
            Request request,
            ICommandHandler<CreateRecipeDirectionCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateRecipeDirectionCommand(
                request.RecipeId,
                request.StepNumber,
                request.Description,
                request.ImageUrl is not null ? new Uri(request.ImageUrl) : null
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.RecipeDirections);
    }

    internal sealed class Request
    {
        public Guid RecipeId { get; init; }
        public int StepNumber { get; init; }
        public string Description { get; init; } = string.Empty;
        public string? ImageUrl { get; init; }
    }
}
