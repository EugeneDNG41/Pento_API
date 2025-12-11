using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.Accept;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class AcceptTradeRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/offers/{tradeOfferId:guid}/requests/{tradeRequestId:guid}/accept", async (
            Guid tradeOfferId,
            Guid tradeRequestId,
            ICommandHandler<AcceptTradeRequestCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new AcceptTradeRequestCommand(tradeOfferId, tradeRequestId);
            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match((id) =>
                Results.Ok(new {tradeSessionId = id}),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
}
