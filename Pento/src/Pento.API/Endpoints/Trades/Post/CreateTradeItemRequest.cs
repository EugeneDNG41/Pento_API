using NetTopologySuite.Geometries;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades;
using Pento.Application.Trades.Requests.Create;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class CreateMultipleTradeItemRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/requests/items", async (
            Request request,
            ICommandHandler<CreateTradeItemRequestCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CreateTradeItemRequestCommand(
                TradeOfferId: request.TradeOfferId,
                Location: request.Location,
                Items: request.Items.Select(i =>
                    new AddTradeItemDto(
                        i.FoodItemId,
                        i.Quantity,
                        i.UnitId
                    )
                ).ToList()
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/trades/requests/{id}", id),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Trades);
    }

    internal sealed class Request
    {
        public Guid TradeOfferId { get; init; }
        public Point Location { get; init; }
        public List<ItemRequest> Items { get; init; } = new();
    }

    internal sealed class ItemRequest
    {
        public Guid FoodItemId { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
