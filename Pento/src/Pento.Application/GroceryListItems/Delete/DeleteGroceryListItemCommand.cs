using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListItems.Delete;

public sealed record DeleteGroceryListItemCommand(Guid Id) : ICommand<Guid>;
