using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Update;

public sealed record UpdateGroceryListCommand(
    Guid Id,
    string Name
) : ICommand<Guid>;
