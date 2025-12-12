using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades;
using Pento.Application.Trades.Offers.UpdateItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Patch;

internal sealed class UpdateTradeOfferItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/offers/{tradeOfferId:guid}/items", async (
            Guid tradeOfferId,
            Request request,
            ICommandHandler<UpdateTradeOfferItemsCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new UpdateTradeOfferItemsCommand(tradeOfferId, request.Items);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                Results.NoContent,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public List<UpdateTradeItemDto> Items { get; init; }
    }
}
