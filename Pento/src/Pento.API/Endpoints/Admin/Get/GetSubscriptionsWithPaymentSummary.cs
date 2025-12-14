using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Subscriptions.GetSummary;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetSubscriptionsWithPaymentSummary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/subscriptions/payment-summary", async (
            Guid[]? subscriptionIds,
            DateOnly? fromDate,
            DateOnly? toDate,
            bool? isActive,
            bool? isDeleted,
            TimeWindow? timeWindow,
            IQueryHandler<GetSubscriptionsWithPaymentSummaryQuery, IReadOnlyList<SubscriptionWithPaymentSummary>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionsWithPaymentSummaryQuery(subscriptionIds, fromDate, toDate, timeWindow, isActive, isDeleted);
            Result<IReadOnlyList<SubscriptionWithPaymentSummary>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManagePayments).WithTags(Tags.Admin);
    }
}
