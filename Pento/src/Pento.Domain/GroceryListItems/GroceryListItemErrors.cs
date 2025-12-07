using Pento.Domain.Abstractions;

namespace Pento.Domain.GroceryListItems;

public static class GroceryListItemErrors
{
    public static readonly Error NotFound = Error.NotFound(
    code: "GroceryListItem.NotFound",
    description: "The grocery list could not be found."
);
    public static readonly Error ForbiddenAccess = Error.Forbidden(
        code: "GroceryListItem.Forbidden",
        description: "You do not have permission to modify this grocery list item."
    );
    public static readonly Error InvalidPriority = Error.Conflict(
        code: "GroceryListItem.InvalidPriority",
        description: "Invalid Priority"
    );
}
