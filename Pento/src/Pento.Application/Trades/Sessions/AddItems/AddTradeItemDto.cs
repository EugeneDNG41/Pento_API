namespace Pento.Application.Trades.Sessions.AddItems;

public sealed record AddTradeItemDto(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId
);
