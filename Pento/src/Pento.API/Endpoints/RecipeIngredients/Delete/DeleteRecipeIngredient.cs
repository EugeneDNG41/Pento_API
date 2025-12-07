using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.Delete;
using Pento.Domain.Abstractions;
namespace Pento.API.Endpoints.RecipeIngredients.Delete;

internal sealed class DeleteRecipeIngredient : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("recipe-ingredients/{recipeIngredientId:guid}", async (
            Guid recipeIngredientId,
            ICommandHandler<DeleteRecipeIngredientCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteRecipeIngredientCommand(recipeIngredientId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.RecipeIngredients);
    }


}
