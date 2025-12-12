using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.Create;

public sealed record CreateTradeItemRequestCommand(
    Guid TradeOfferId,
    List<CreateTradeItemRequestDto> Items
) : ICommand<Guid>;
public sealed record CreateTradeItemRequestDto(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId
);




