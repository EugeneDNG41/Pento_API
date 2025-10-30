using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Recipes;
public static class RecipeErrors
{
    public static readonly Error NotFound = Error.NotFound(
       code: "Recipe.IdentityNotFound",
       description: "The specified recipe was not found."
   );

    public static readonly Error InvalidTitle = Error.Conflict(
        code: "Recipe.InvalidTitle",
        description: "The recipe title cannot be empty."
    );

    public static readonly Error InvalidTime = Error.Conflict(
        code: "Recipe.InvalidTime",
        description: "Preparation and cooking time must be non-negative."
    );

    public static readonly Error UnauthorizedAccess = Error.Conflict(
        code: "Recipe.UnauthorizedAccess",
        description: "You are not authorized to modify this recipe."
    );
    public static readonly Error InvalidServings = Error.Conflict(
        code: "Recipe.InvalidServings",
        description: "Servings must be greater than zero."
    );

    public static readonly Error InvalidDifficulty = Error.Conflict(
        code: "Recipe.InvalidDifficulty",
        description: "The provided difficulty level is invalid."
    );
}
