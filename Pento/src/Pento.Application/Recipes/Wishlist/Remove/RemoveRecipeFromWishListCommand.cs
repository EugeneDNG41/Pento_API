using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Wishlist.Remove;

public sealed record RemoveRecipeFromWishListCommand(Guid RecipeId)
    : ICommand<Guid>;
