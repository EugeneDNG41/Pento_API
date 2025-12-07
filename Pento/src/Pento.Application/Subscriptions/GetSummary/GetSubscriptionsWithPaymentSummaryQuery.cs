using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Subscriptions.GetSummary;

public sealed record GetSubscriptionsWithPaymentSummaryQuery(
    Guid[]? SubscriptionIds,
    DateOnly? FromDate,
    DateOnly? ToDate,
    TimeWindow? TimeWindow,
    bool? IsActive,
    bool? IsDeleted) : IQuery<IReadOnlyList<SubscriptionWithPaymentSummary>>;
