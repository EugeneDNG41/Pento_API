using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades;
using Pento.Application.Trades.Offers.AddItems;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class AddTradeOfferItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/offers/{tradeOfferId:guid}/items", async (
            Guid tradeOfferId,
            Request request,
            ICommandHandler<AddTradeOfferItemsCommand, IReadOnlyList<TradeItemResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new AddTradeOfferItemsCommand(tradeOfferId, request.Items);
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
