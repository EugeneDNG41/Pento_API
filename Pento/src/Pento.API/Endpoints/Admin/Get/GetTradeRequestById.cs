using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.AdminGetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetTradeRequestById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/trades/requests/{tradeRequestId:guid}", async (
            Guid tradeRequestId,
            IQueryHandler<GetAdminTradeRequestByIdQuery, TradeRequestDetailAdminResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAdminTradeRequestByIdQuery(tradeRequestId);
            Result<TradeRequestDetailAdminResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
