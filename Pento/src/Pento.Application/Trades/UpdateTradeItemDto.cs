namespace Pento.Application.Trades;

public sealed record UpdateTradeItemDto(
    Guid TradeItemId,
    decimal Quantity,
    Guid UnitId
);
