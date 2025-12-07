namespace Pento.Application.FoodItemLogs.GetSummary;

public sealed record FoodSummary(
    string WeightUnit,
    string VolumeUnit,
    FoodItemLogSummary LogSummary,
    FoodItemSummary FoodItemSummary);
public sealed record FoodItemLogSummary
{
    public decimal IntakeByWeight { get; init; }
    public decimal IntakeByVolume { get; init; }
    public decimal ConsumptionByWeight { get; init; }
    public decimal ConsumptionByVolume { get; init; }
    public decimal DiscardByWeight { get; init; }
    public decimal DiscardByVolume { get; init; }
}
public sealed record FoodItemSummary
{
    public int TotalFoodItems { get; init; }
    public int FreshCount { get; init; }
    public int ExpiringCount { get; init; }
    public int ExpiredCount { get; init; }
    public decimal FreshByWeight { get; init; }
    public decimal FreshByVolume { get; init; }
    public decimal ExpiringByWeight { get; init; }
    public decimal ExpiringByVolume { get; init; }
    public decimal ExpiredByWeight { get; init; }
    public decimal ExpiredByVolume { get; init; }
}
public sealed record ReservationSummary
{
    public int TotalReservations { get; init; }
    public int PendingCount { get; init; }
    public int FulfilledCount { get; init; }
    public int CancelledCount { get; init; }
    public decimal PendingByWeight { get; init; }
    public decimal PendingByVolume { get; init; }
    public decimal FulfilledByWeight { get; init; }
    public decimal FulfilledByVolume { get; init; }
    public decimal CancelledByWeight { get; set; }
    public decimal CancelledByVolume { get; init; }
}
