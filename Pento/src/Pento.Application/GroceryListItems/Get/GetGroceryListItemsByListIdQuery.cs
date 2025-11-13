using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryListItems.Get;

public sealed record GetGroceryListItemsByListIdQuery(Guid ListId)
    : IQuery<IReadOnlyList<GroceryListItemResponse>>;
