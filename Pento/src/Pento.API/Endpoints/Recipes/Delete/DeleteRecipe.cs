using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Delete;

internal sealed class DeleteRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("recipes/{id:guid}",
            async (Guid id, ICommandHandler<DeleteRecipeCommand> handler, CancellationToken cancellationToken) =>
            {
                var command = new DeleteRecipeCommand(id);

                Result result = await handler.Handle(command, cancellationToken);

                return result;
            })
        .WithTags(Tags.Recipes);
    }
}
