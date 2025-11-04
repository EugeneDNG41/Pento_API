using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.Delete;
using Pento.Application.RecipeIngredients.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
namespace Pento.API.Endpoints.RecipeIngredients.Put;

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
