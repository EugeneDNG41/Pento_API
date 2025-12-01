using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Wishlist.Add;

public sealed record AddRecipeToWishListCommand(Guid RecipeId)
    : ICommand<Guid>;
