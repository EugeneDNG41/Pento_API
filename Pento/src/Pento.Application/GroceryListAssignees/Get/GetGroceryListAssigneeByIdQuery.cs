using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListAssignees.Get;

public sealed record GetGroceryListAssigneeByIdQuery(Guid Id)
    : IQuery<GroceryListAssigneeResponse>;
