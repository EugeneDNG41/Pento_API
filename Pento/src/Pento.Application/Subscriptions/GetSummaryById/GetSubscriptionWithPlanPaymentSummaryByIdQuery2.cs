using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.GetSummaryById;

namespace Pento.Application.Payments.GetSummaryById;

public sealed record GetSubscriptionWithPlanPaymentSummaryByIdQuery2(
    Guid SubscriptionId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IQuery<SubscriptionWithPlanPaymentSummary2>;
