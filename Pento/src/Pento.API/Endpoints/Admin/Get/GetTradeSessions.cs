using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Sessions.AdminGetAll;
using Pento.Application.Trades.Sessions.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetTradeSessions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/trades/sessions", async (
            Guid? offerId,
            Guid? requestId,
            TradeSessionStatus? status,
            SortOrder? sortOrder,
            IQueryHandler<GetAdminTradeSessionsQuery, PagedList<TradeSessionBasicResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10
        ) =>
        {
            var query = new GetAdminTradeSessionsQuery(offerId, requestId, status, sortOrder, pageNumber, pageSize);
            Result<PagedList<TradeSessionBasicResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        }).RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
