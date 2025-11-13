using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListAssignees.Create;

public sealed record CreateGroceryListAssigneeCommand(
    Guid GroceryListId,
    Guid HouseholdMemberId
) : ICommand<Guid>;
