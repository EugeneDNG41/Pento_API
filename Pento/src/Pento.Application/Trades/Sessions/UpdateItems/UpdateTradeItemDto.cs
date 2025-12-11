namespace Pento.Application.Trades.Sessions.UpdateItems;

public sealed record UpdateTradeItemDto(
    Guid TradeItemId,
    decimal Quantity,
    Guid UnitId
);
