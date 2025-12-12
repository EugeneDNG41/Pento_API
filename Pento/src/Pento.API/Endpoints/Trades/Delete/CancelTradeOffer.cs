using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Offers.Cancel;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class CancelTradeOffer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/offers/{tradeOfferId:guid}/cancel", async (
            Guid tradeOfferId,
            ICommandHandler<CancelTradeOfferCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CancelTradeOfferCommand(tradeOfferId);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                Results.NoContent,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
}
