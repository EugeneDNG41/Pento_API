using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.GetSummaryById;

public sealed record GetSubscriptionWithPlanPaymentSummaryByIdQuery2(
    Guid SubscriptionId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IQuery<SubscriptionWithPlanPaymentSummary2>;
