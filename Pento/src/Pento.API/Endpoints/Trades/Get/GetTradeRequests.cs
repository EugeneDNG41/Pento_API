using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.TradeItems.Requests.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetTradeRequestsByOffer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trade-requests/offer/{offerId:guid}", async (
            Guid offerId,
            IQueryHandler<GetTradeRequestsByOfferQuery, IReadOnlyList<TradeRequestResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetTradeRequestsByOfferQuery(offerId);

            Result<IReadOnlyList<TradeRequestResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Trades);
    }
}
