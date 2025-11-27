
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Subscriptions.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Get;

internal sealed class GetSubscriptions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions", async (
            string? searchTerm,
            bool? isActive,
            IQueryHandler<GetSubscriptionsQuery, PagedList<SubscriptionDetailResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<SubscriptionDetailResponse>> result = await handler.Handle(new GetSubscriptionsQuery(searchTerm, isActive, pageNumber, pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
}
