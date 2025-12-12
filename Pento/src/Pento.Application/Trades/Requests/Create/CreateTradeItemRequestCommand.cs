using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.Create;

public sealed record CreateTradeItemRequestCommand(
    Guid TradeOfferId,
    List<AddTradeItemDto> Items
) : ICommand<Guid>;





