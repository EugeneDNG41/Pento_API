using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListAssignees.Get;

public sealed record GetGroceryListAssigneesByListIdQuery(Guid GroceryListId)
    : IQuery<IReadOnlyList<GroceryListAssigneeResponse>>;
