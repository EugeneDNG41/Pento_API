using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Get;

public sealed record GetGroceryListsByHouseholdIdQuery()
    : IQuery<IReadOnlyList<GroceryListResponse>>;
