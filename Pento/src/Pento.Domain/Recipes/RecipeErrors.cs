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

    public static readonly Error UnauthorizedAccess = Error.Conflict(
        code: "Recipe.UnauthorizedAccess",
        description: "You are not authorized to modify this recipe."
    );

}
