using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItemLogs.GetSummary;

public sealed record GetFoodItemLogSummaryQuery(
    DateTime? FromUtc,
    DateTime? ToUtc,
    Guid? WeightUnitId,
    Guid? VolumeUnitId) : IQuery<FoodItemLogSummary>;
