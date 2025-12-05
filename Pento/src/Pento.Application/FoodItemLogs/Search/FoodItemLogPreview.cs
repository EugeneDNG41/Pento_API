using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItemLogs.GetSummary;
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
public sealed record FoodItemLogBasicSummary
{
    public List<FoodItemLogBasicResponse> LogsByWeight { get; init; } = new();
    public List<FoodItemLogBasicResponse> LogsByVolume { get; init; } = new();
}
public sealed record FoodItemLogBasicResponse
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public FoodItemLogAction Action { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; }  
}
public sealed record GetFoodItemLogBasicSummaryQuery(
    DateOnly? FromUtc,
    DateOnly? ToUtc,
    Guid? WeightUnitId,
    Guid? VolumeUnitId) : IQuery<FoodSummary>;
