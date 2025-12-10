using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.TradeItem.Offers.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class CreateMultipleTradeItemOffer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trade-offers/items", async (
            Request request,
            ICommandHandler<CreateTradeItemOfferCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTradeItemOfferCommand(
                request.StartDate,
                request.EndDate,
                request.PickupOption,
                [.. request.Items.Select(i =>
                    new CreateTradeItemOfferDto(i.FoodItemId, i.Quantity, i.UnitId)
                )]
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/trade-offers/{id}", id),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Trades)
        .WithDescription("PickupOption\r\n{\r\n  InPerson =1,\r\n    Delivery =2,\r\n    Flexible =3\r\n}");
    }

    internal sealed class Request
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public PickupOption PickupOption { get; init; }
        public List<ItemRequest> Items { get; init; } = new();
    }

    internal sealed class ItemRequest
    {
        public Guid FoodItemId { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
