using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItemLogs.GetSummary;

public sealed record GetAdminFoodItemLogSummaryQuery(
    Guid? HouseholdId,
    DateTime? FromUtc,
    DateTime? ToUtc,
    Guid? WeightUnitId,
    Guid? VolumeUnitId,
    bool? IsDeleted) : IQuery<FoodSummary>;
