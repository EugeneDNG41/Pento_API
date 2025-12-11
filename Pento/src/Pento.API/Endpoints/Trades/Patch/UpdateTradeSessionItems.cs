using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Application.Trades.Sessions.UpdateItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class UpdateTradeSessionItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/sessions/{tradeSessionId:guid}/items", async (
            Guid tradeSessionId,
            Request request,
            ICommandHandler<UpdateTradeSessionItemsCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new UpdateTradeSessionItemsCommand(tradeSessionId, request.Items);
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
