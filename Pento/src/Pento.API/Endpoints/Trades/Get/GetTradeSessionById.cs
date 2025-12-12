using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetTradeSessionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trades/sessions/{sessionId:guid}", async (
            Guid sessionId,
            IQueryHandler<GetTradeSessionByIdQuery, TradeSessionDetailResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetTradeSessionByIdQuery(sessionId);
            Result<TradeSessionDetailResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        }).RequireAuthorization()
        .WithTags(Tags.Trades);
    }
}
