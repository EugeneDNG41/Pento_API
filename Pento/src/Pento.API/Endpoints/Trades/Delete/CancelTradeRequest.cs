using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.Cancel;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class CancelTradeRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/requests/{tradeRequestId:guid}/cancel", async (
            Guid tradeRequestId,
            ICommandHandler<CancelTradeRequestCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CancelTradeRequestCommand(tradeRequestId);
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
