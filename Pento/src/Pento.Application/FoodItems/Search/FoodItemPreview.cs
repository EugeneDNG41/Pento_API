using Pento.Application.FoodReferences.Get;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.Search;

public sealed record FoodItemPreviewRow(
    Guid Id,
    string Name,
    FoodGroup FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateOnly ExpirationDate
);
public sealed record FoodItemPreview(
    Guid Id,
    string Name,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateOnly ExpirationDate
);
public sealed class PagedFoodItemPreviewResponse
{
    public IReadOnlyList<FoodItemPreview> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}
