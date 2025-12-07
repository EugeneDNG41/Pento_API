using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeWishLists;

public static class RecipeWishListErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "RecipeWishList.NotFound",
        "The specified recipe wish list was not found."
    );
    public static readonly Error AlreadyExists = Error.Conflict(
        "RecipeWishList.AlreadyExists",
        "The specified recipe wish list already exists."
    );
}
