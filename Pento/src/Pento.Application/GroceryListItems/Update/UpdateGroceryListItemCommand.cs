using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListItems.Update;

public sealed record UpdateGroceryListItemCommand(
    Guid Id,
    decimal Quantity,
    string? Notes,
    string? CustomName,
    decimal? EstimatedPrice,
    string Priority
) : ICommand<Guid>;
