using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Wishlist.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetRecipeWishList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes/wishlist", async (
            IQueryHandler<GetRecipeWishListQuery, List<RecipeWishListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeWishListQuery();

            Result<List<RecipeWishListResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                list => Results.Ok(list),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes)
        .RequireAuthorization();
    }
}
