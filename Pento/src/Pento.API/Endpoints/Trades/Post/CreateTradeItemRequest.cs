using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.TradeItem.Requests.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Request;

internal sealed class CreateMultipleTradeItemRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trade-requests/items", async (
            Request request,
            ICommandHandler<CreateTradeItemRequestCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CreateTradeItemRequestCommand(
                TradeOfferId: request.TradeOfferId,
                Items: request.Items.Select(i =>
                    new CreateTradeItemRequestDto(
                        i.FoodItemId,
                        i.Quantity,
                        i.UnitId
                    )
                ).ToList()
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/trade-requests/{id}", id),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Trades);
    }

    internal sealed class Request
    {
        public Guid TradeOfferId { get; init; }
        public List<ItemRequest> Items { get; init; } = new();
    }

    internal sealed class ItemRequest
    {
        public Guid FoodItemId { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
