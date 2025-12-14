using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.Reject;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Patch;

internal sealed class RejectTradeRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/requests/{tradeRequestId:guid}/reject", async (
            Guid tradeRequestId,
            ICommandHandler<RejectTradeRequestCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new RejectTradeRequestCommand(tradeRequestId);
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
