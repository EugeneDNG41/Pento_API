using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.CreateDetail;

public sealed record CreateGroceryListDetailCommand(
    string Name,
    List<GroceryListItemRequest> Items,
    List<GroceryListAssigneeRequest> Assignees
) : ICommand<Guid>;

public sealed record GroceryListItemRequest(
    Guid? FoodRefId,
    decimal Quantity,
    string? CustomName,
    Guid? UnitId,
    decimal? EstimatedPrice,
    string? Notes,
    string Priority
);

public sealed record GroceryListAssigneeRequest(
    Guid HouseholdMemberId
);
