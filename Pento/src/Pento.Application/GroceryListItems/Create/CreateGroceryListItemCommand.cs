using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListItems.Create;

public sealed record CreateGroceryListItemCommand(
    Guid ListId,
    Guid FoodRefId,
    decimal Quantity,
    string? CustomName,
    Guid? UnitId,
    decimal? EstimatedPrice,
    string? Notes,
    string Priority
) : ICommand<Guid>;
