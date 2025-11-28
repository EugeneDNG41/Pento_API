namespace Pento.Application.FoodItemLogs.GetSummary;

public sealed record FoodItemLogSummary
{
    public decimal TotalIntakeByWeight { get; init; }
    public decimal TotalIntakeByVolume { get; init; }
    public decimal TotalConsumptionByWeight { get; init; }
    public decimal TotalConsumptionByVolume { get; init; }
    public decimal TotalDiscardByWeight { get; init; }
    public decimal TotalDiscardByVolume { get; init; }
    public string WeightUnit { get; init; }
    public string VolumeUnit { get; init; }
} //add total expiring, available, expired, reserved
public sealed record FoodItemSummary //filter by food group
{
    public decimal TotalFreshByWeight { get; init; }
    public decimal TotalFreshByVolume { get; init; }
    public decimal TotalExpiringByWeight { get; init; }
    public decimal TotalExpiringByVolume { get; init; }
    public decimal TotalExpiredByWeight { get; init; }
    public decimal TotalExpiredByVolume { get; init; }
}
public sealed record ReservationSummary //filter by type
{
    public decimal TotalPendingByWeight { get; init; }
    public decimal TotalPendingByVolume { get; init; }
    public decimal TotalFulfilledByWeight { get; init; }
    public decimal TotalFulfilledByVolume { get; init; }   
    public decimal TotalCancelledByWeight { get; set; }
    public decimal TotalCancelledByVolume { get; init; }
}
