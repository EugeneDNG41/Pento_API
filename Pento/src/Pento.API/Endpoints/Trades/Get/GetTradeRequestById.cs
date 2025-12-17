using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.GetAll;
using Pento.Application.Trades.Requests.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetTradeRequestById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trades/requests/{tradeRequestId:guid}", async (
            Guid tradeRequestId,
            IQueryHandler<GetTradeRequestByIdQuery, TradeRequestDetailResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetTradeRequestByIdQuery(tradeRequestId);
            Result<TradeRequestDetailResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Trades);
    }
}

