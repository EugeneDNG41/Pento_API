using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Subscriptions.GetSummaryById;

public sealed record GetSubscriptionWithPlanPaymentSummaryByIdQuery(
    Guid SubscriptionId,
    DateOnly? FromDate, 
    DateOnly? ToDate,
    TimeWindow? TimeWindow) : IQuery<SubscriptionWithPlanPaymentSummary>;
