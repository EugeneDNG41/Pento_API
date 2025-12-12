namespace Pento.Application.Trades;

public sealed record AddTradeItemDto(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId
);
