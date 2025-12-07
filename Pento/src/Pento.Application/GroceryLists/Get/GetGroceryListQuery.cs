using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Get;

public sealed record GetGroceryListQuery(Guid Id)
    : IQuery<GroceryListDetailResponse>;
