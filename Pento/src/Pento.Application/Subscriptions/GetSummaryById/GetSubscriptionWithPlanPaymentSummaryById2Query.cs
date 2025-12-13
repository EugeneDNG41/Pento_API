using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.GetSummaryById;

public sealed record GetSubscriptionWithPlanPaymentSummaryById2Query(
    Guid SubscriptionId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IQuery<SubscriptionWithPlanPaymentSummary2>;
