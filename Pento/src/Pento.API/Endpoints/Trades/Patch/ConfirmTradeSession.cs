using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.Confirm;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class ConfirmTradeSession : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/sessions/{tradeSessionId:guid}/confirm", async (
            Guid tradeSessionId,
            ICommandHandler<ConfirmTradeSessionCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new ConfirmTradeSessionCommand(tradeSessionId);
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
