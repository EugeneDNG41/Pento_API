using Pento.Application.FoodReferences.Get;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.Search;

public sealed record FoodItemPreviewRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public FoodGroup FoodGroup { get; init; }
    public Uri? ImageUrl { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; }
    public DateOnly ExpirationDate { get; init; }
}
public sealed record FoodItemPreview(
    Guid Id,
    string Name,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateOnly ExpirationDate
);
