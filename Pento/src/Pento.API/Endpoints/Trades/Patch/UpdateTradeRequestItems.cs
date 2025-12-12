using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades;
using Pento.Application.Trades.Requests.UpdateItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Patch;

internal sealed class UpdateTradeRequestItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/requests/{tradeRequestId:guid}/items", async (
            Guid tradeRequestId,
            Request request,
            ICommandHandler<UpdateTradeRequestItemsCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new UpdateTradeRequestItemsCommand(tradeRequestId, request.Items);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                Results.NoContent,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public List<UpdateTradeItemDto> Items { get; init; }
    }
}
