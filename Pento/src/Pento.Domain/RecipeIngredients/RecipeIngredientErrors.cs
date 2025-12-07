using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeIngredients;

public static class RecipeIngredientErrors
{
    public static Error NotFound =>
        Error.NotFound(
            "RecipeIngredients.IdentityNotFound",
            $"The recipe ingredient was not found."
        );

    public static Error NotFoundByRecipe(Guid recipeId) =>
        Error.NotFound(
            "RecipeIngredients.NotFoundByRecipe",
            $"No recipe ingredients were found for recipe '{recipeId}'."
        );
    public static readonly Error Conflict = Error.Conflict(
        "RecipeIngredients.Conflict",
        "A recipe ingredient with the same food reference and unit already exists for this recipe."
    );

    public static readonly Error Unauthorized = Error.Problem(
        "RecipeIngredients.Unauthorized",
        "You are not authorized to modify this recipe ingredient."
    );
}

