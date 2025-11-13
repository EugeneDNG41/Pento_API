using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListAssignees.Update;

public sealed record UpdateGroceryListAssigneeCommand(
    Guid Id,
    bool IsCompleted
) : ICommand;
