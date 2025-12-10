using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.Accept;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class RemoveTradeSessionItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trade-sessions/{tradeSessionId:guid}/items", async (
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
