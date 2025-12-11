using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class AddTradeSessionItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/sessions/{tradeSessionId:guid}/items", async (
            Guid tradeSessionId,
            Request request,
            ICommandHandler<AddTradeSessionItemsCommand, IReadOnlyList<TradeItemResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new AddTradeSessionItemsCommand(tradeSessionId, request.Items);
            Result<IReadOnlyList<TradeItemResponse>> result = await handler.Handle(command, cancellationToken);
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
        public List<AddTradeItemDto> Items { get; init; }
    }
}
