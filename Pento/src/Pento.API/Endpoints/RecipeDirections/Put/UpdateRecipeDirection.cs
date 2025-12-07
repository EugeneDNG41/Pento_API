using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeDirections.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.RecipeDirections.Put;

internal sealed class UpdateRecipeDirection : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("recipe-directions/{directionId:guid}", async (
            Guid directionId,
            Request request,
            ICommandHandler<UpdateRecipeDirectionCommand> handler,
            CancellationToken cancellationToken) =>
        {

            Result result = await handler.Handle(
                new UpdateRecipeDirectionCommand(directionId, request.Description),
                cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.RecipeDirections);
    }

    internal sealed class Request
    {
        public string Description { get; init; } = string.Empty;
    }
}
