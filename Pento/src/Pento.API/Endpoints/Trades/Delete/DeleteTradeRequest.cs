using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class DeleteTradeRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/requests/{tradeRequestId:guid}", async (
            Guid tradeRequestId,
            ICommandHandler<DeleteTradeRequestCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new DeleteTradeRequestCommand(tradeRequestId);
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
