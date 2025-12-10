using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.TradeItem.Offers.Create;

public sealed record CreateTradeItemOfferCommand(
    DateTime StartDate,
    DateTime EndDate,
    PickupOption PickupOption,
    List<CreateTradeItemOfferDto> Items
) : ICommand<Guid>;

public sealed record CreateTradeItemOfferDto(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId
);
