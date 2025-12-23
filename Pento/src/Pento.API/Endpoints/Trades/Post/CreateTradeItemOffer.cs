using NetTopologySuite.Geometries;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades;
using Pento.Application.Trades.Offers.Create;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class CreateMultipleTradeItemOffer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/offers/items", async (
            Request request,
            ICommandHandler<CreateTradeItemOfferCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTradeItemOfferCommand(
                request.StartDate,
                request.EndDate,
                request.PickupOption,
                request.Location,
                [.. request.Items.Select(i =>
                    new AddTradeItemDto(i.FoodItemId, i.Quantity, i.UnitId)
                )]
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/trades/offers/{id}", new { id }),
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
