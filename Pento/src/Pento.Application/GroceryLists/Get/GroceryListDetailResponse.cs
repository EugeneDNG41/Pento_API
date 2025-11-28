namespace Pento.Application.GroceryLists.Get;

public sealed record GroceryListDetailResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc,
    IReadOnlyList<GroceryListItemResponse> Items,
    IReadOnlyList<GroceryListAssigneeResponse> Assignees
);

public sealed record GroceryListItemResponse(
    Guid Id,
    Guid FoodRefId,
    string FoodRefName,
    Uri? ImageUrl,
    decimal Quantity,
    string? CustomName,
    Guid? UnitId,
    string? UnitAbbreviation,
    string? Notes,
    string Priority,
    Guid AddedBy,
    DateTime CreatedOnUtc
);

public sealed record GroceryListAssigneeResponse(
    Guid Id,
    Guid HouseholdMemberId,
    bool IsCompleted,
    DateTime AssignedOnUtc
);
