using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.Cancel;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class CancelTradeSession : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/sessions/{tradeSessionId:guid}/cancel", async (
            Guid tradeSessionId,
            ICommandHandler<CancelTradeSessionCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CancelTradeSessionCommand(tradeSessionId);
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
