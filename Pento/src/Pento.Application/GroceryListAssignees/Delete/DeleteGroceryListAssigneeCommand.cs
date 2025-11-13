using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListAssignees.Delete;

public sealed record DeleteGroceryListAssigneeCommand(Guid Id) : ICommand<Guid>;
