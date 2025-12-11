using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Activities.GetSummary;

public sealed record GetActivitySummaryQuery(
    string[]? Codes,
    Guid[]? UserIds,
    Guid[]? HouseholdIds,
    DateOnly? FromDate,
    DateOnly? ToDate,
    TimeWindow? TimeWindow) : IQuery<IReadOnlyList<ActivitySummary>>;

