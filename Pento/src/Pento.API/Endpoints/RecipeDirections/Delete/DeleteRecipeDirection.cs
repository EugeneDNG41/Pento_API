using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeDirections.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.RecipeDirections.Delete;

internal sealed class DeleteRecipeDirection : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("recipe-directions/{recipeDirectionId:guid}", async (
            Guid recipeDirectionId,
            ICommandHandler<DeleteRecipeDirectionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new DeleteRecipeDirectionCommand(recipeDirectionId),
                cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.RecipeDirections);
    }
}
