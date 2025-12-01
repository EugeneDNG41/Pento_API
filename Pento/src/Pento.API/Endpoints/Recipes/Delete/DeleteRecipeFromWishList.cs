using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Wishlist.Remove;
using Pento.Application.RecipeWishLists.Remove;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Delete;

internal sealed class DeleteRecipeFromWishList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("recipes/{recipeId:guid}/wishlist", async (
            Guid recipeId,
            ICommandHandler<RemoveRecipeFromWishListCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveRecipeFromWishListCommand(recipeId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes)
        .RequireAuthorization();
    }
}
