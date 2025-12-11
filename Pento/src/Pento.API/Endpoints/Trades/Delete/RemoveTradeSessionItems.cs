using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.RemoveItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class RemoveTradeSessionItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/sessions/{tradeSessionId:guid}/items", async (
            Guid tradeSessionId,
            Guid[] tradeItemIds,
            ICommandHandler<RemoveTradeSessionItemsCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new RemoveTradeSessionItemsCommand(tradeSessionId, tradeItemIds);
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
