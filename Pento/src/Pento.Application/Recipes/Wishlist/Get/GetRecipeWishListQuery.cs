using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeWishLists.GetAll;
namespace Pento.Application.Recipes.Wishlist.Get;
public sealed record GetRecipeWishListQuery()
    : IQuery<List<RecipeWishListResponse>>;
