using Pento.Domain.Recipes;

namespace Pento.Application.RecipeWishLists.GetAll;

public sealed record RecipeWishListResponse(
    Guid WishListId,
    Guid RecipeId,
    string Title,
    Uri? ImageUrl,
    DifficultyLevel? DifficultyLevel,
    int PrepTimeMinutes,
    DateTime AddedOnUtc
);
