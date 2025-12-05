using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Payments.GetSummary;

namespace Pento.Application.Payments.GetSummaryById;

public sealed record GetSubscriptionWithPlanPaymentSummaryByIdQuery(
    Guid SubscriptionId,
    DateOnly? FromDate, 
    DateOnly? ToDate,
    TimeWindow? TimeWindow) : IQuery<SubscriptionWithPlanPaymentSummary>;
