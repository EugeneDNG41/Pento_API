using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GroceryLists;
public static class GroceryListErrors
{
    public static readonly Error NotFound = Error.NotFound(
    code: "GroceryListAssignee.NotFound",
    description: "The grocery list could not be found."
);

    public static readonly Error DuplicateName = Error.Conflict(
        code: "GroceryList.DuplicateNamet",
        description: "Other household grocery list with the same name has already been created"
    );

    public static readonly Error ForbiddenAccess = Error.Forbidden(
        code: "GroceryList.Forbidden",
        description: "You do not have permission to modify this grocery list."
    );
    public static readonly Error NotFoundByHousehold = Error.NotFound(
        code: "GroceryList.NotFoundByHousehold",
        description: "No grocery lists were found for this household."
    );
}
