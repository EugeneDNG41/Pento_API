namespace Pento.Application.FoodItemLogs.GetSummary;

public sealed record FoodItemLogSummary
{
    public decimal TotalIntakeByWeight { get; init; }
    public decimal TotalIntakeByVolume { get; init; }
    public decimal TotalConsumptionByWeight { get; init; }
    public decimal TotalConsumptionByVolume { get; init; }
    public decimal TotalDiscardByWeight { get; init; }
    public decimal TotalDiscardByVolume { get; init; }
    public decimal CurrentStockByWeight { get; init; }
    public decimal CurrentStockByVolume { get; init; }
    public decimal CurrentAvailableByWeight { get; init; }
    public decimal CurrentAvailableByVolume { get; init; }
    public decimal CurrentReservedByWeight { get; init; }
    public decimal CurrentReservedByVolume { get; init; }
    public decimal CurrentExpiringByWeight { get; init; }
    public decimal CurrentExpiringByVolume { get; init; }
    public string WeightUnit { get; init; }
    public string VolumeUnit { get; init; }
} //add total expiring, available, expired, reserved
