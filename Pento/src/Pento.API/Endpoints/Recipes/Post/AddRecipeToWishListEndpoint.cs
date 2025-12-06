using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Wishlist.Add;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Post;

internal sealed class AddRecipeToWishList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes/{recipeId:guid}/add-to-wishlist", async (
            Guid recipeId,
            ICommandHandler<AddRecipeToWishListCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddRecipeToWishListCommand(
                recipeId
            );

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
