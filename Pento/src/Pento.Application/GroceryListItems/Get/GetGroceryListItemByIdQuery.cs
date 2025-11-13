using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListItems.Get;

public sealed record GetGroceryListItemByIdQuery(Guid Id)
    : IQuery<GroceryListItemResponse>;
