using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Application.Trades.Requests.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetTradeRequests : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/trades/requests", async (
            Guid? offerId,
            TradeRequestStatus? status,
            bool? isDeleted,
            GetTradeRequestsSortBy? sortBy,
            SortOrder? sortOrder,
            IQueryHandler<GetAdminTradeRequestsQuery, PagedList<TradeRequestAdminResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10
        ) =>
        {
            var query = new GetAdminTradeRequestsQuery(offerId, status, isDeleted, sortBy, sortOrder, pageNumber, pageSize);
            Result<PagedList<TradeRequestAdminResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
