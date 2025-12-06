using Pento.Application.Abstractions.Messaging;
namespace Pento.Application.Recipes.Wishlist.Get;
public sealed record GetRecipeWishListQuery()
    : IQuery<List<RecipeWishListResponse>>;
