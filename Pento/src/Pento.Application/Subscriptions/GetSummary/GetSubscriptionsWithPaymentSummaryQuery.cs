using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Subscriptions.GetSummary;

namespace Pento.Application.Payments.GetSummary;

public sealed record GetSubscriptionsWithPaymentSummaryQuery(
    Guid[]? SubscriptionIds,
    DateOnly? FromDate, 
    DateOnly? ToDate,
    TimeWindow? TimeWindow,
    bool? IsActive, 
    bool? IsDeleted) : IQuery<IReadOnlyList<SubscriptionWithPaymentSummary>>;
