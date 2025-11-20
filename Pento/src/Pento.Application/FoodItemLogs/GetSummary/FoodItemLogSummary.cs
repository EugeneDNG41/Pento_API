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
