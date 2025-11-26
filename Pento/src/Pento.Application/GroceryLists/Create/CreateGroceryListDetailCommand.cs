using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.CreateDetail;

public sealed record CreateGroceryListDetailCommand(
    string Name,
    List<GroceryListItemRequest> Items
) : ICommand<Guid>;

public sealed record GroceryListItemRequest(
    Guid? FoodRefId,
    decimal Quantity,
    string? CustomName,
    Guid? UnitId,
    string? Notes,
    string Priority
);

