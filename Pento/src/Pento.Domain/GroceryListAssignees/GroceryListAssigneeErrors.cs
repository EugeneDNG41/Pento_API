using Pento.Domain.Abstractions;

namespace Pento.Domain.GroceryListAssignees;

public static class GroceryListAssigneeErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "GroceryListAssignee.NotFound",
        description: "The grocery list assignee could not be found."
    );

    public static readonly Error DuplicateAssignment = Error.Conflict(
        code: "GroceryListAssignee.DuplicateAssignment",
        description: "This household member has already been assigned to the grocery list."
    );

    public static readonly Error ForbiddenAccess = Error.Forbidden(
        code: "GroceryListAssignee.Forbidden",
        description: "You do not have permission to modify this grocery list assignee."
    );

    public static readonly Error AlreadyCompleted = Error.Conflict(
        code: "GroceryListAssignee.AlreadyCompleted",
        description: "This grocery list assignee is already marked as completed."
    );

    public static Error NotFoundByList(Guid listId) => Error.NotFound(
        code: "GroceryListAssignee.NotFoundByList",
        description: $"No assignees were found for grocery list '{listId}'."
    );
    public static Error NotFoundByMember => Error.NotFound(
        code: "GroceryListAssignee.NotFoundByMember",
        description: $"This user does not exist in the current household"
    );
}
