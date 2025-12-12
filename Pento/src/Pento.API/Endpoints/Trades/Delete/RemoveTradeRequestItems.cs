using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.RemoveItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class RemoveTradeRequestItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/requests/{tradeRequestId:guid}/items", async (
            Guid tradeRequestId,
            Guid[] tradeItemIds,
            ICommandHandler<RemoveTradeRequestItemsCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new RemoveTradeRequestItemsCommand(tradeRequestId, tradeItemIds);
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
