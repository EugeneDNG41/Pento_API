using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.SendMessage;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class SendTradeMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trade-sessions/{tradeSessionId:guid}/messages", async (
            Guid tradeSessionId,
            Request request,
            ICommandHandler<SendTradeMessageCommand, TradeMessageResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new SendTradeMessageCommand(tradeSessionId, request.Message);
            Result<TradeMessageResponse> result = await handler.Handle(command, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Message { get; init; } = string.Empty;
    }
}
