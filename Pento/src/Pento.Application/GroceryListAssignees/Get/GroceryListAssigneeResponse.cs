namespace Pento.Application.GroceryListAssignees.Get;

public sealed record GroceryListAssigneeResponse(
    Guid Id,
    Guid GroceryListId,
    Guid HouseholdMemberId,
    bool IsCompleted,
    DateTime AssignedOnUtc
);
