using NetTopologySuite.Geometries;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.Create;

public sealed record CreateTradeItemOfferCommand(
    DateTime StartDate,
    DateTime EndDate,
    PickupOption PickupOption,
    Point location,
    List<AddTradeItemDto> Items
) : ICommand<Guid>;
