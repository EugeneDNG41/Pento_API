namespace Pento.Application.GroceryLists.Get;

public sealed record GroceryListResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
