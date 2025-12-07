using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Delete;

public sealed record DeleteGroceryListCommand(Guid Id) : ICommand<Guid>;
