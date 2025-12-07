using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Create;

public sealed record CreateGroceryListCommand(
    string Name
) : ICommand<Guid>;
