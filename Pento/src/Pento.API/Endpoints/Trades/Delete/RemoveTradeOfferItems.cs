using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Offers.RemoveItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class RemoveTradeOfferItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/offers/{tradeOfferId:guid}/items", async (
            Guid tradeOfferId,
            Guid[] tradeItemIds,
            ICommandHandler<RemoveTradeOfferItemsCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new RemoveTradeOfferItemsCommand(tradeOfferId, tradeItemIds);
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
