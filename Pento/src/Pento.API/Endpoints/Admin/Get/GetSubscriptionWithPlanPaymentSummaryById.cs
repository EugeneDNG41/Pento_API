using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Payments.GetSummaryById;
using Pento.Application.Subscriptions.GetSummaryById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetSubscriptionWithPlanPaymentSummaryById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/subscriptions/{subscriptionId:guid}/payment-summary", async (
            Guid subscriptionId,
            DateOnly? fromDate,
            DateOnly? toDate,
            TimeWindow? timeWindow,
            IQueryHandler<GetSubscriptionWithPlanPaymentSummaryByIdQuery, SubscriptionWithPlanPaymentSummary> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionWithPlanPaymentSummaryByIdQuery(subscriptionId, fromDate, toDate, timeWindow);
            Result<SubscriptionWithPlanPaymentSummary> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
}
internal sealed class GetSubscriptionWithPlanPaymentSummaryById2 : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/subscriptions/{subscriptionId:guid}/v2/payment-summary", async (
            Guid subscriptionId,
            DateOnly? fromDate,
            DateOnly? toDate,
            IQueryHandler<GetSubscriptionWithPlanPaymentSummaryByIdQuery2, SubscriptionWithPlanPaymentSummary2> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionWithPlanPaymentSummaryByIdQuery2(subscriptionId, fromDate, toDate);
            Result<SubscriptionWithPlanPaymentSummary2> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
}
