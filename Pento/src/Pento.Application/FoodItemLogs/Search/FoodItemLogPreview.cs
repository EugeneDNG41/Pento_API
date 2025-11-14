using Pento.Domain.FoodItemLogs;

namespace Pento.Application.FoodItemLogs.Search;

public sealed record FoodItemLogPreview
{
    public Guid Id { get; init; }
    public string FoodItemName { get; init; }
    public Uri? FoodItemImageUrl { get; init; }
    public DateTime Timestamp { get; init; }
    public FoodItemLogAction Action { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; }
}
