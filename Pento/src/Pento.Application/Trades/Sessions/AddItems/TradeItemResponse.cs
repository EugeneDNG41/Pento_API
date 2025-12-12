using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.AddItems;

public sealed record TradeItemResponse(
    Guid TradeItemId,
    Guid FoodItemId,
    string Name,
    string OriginalName,
    string FoodGroup,
    decimal Quantity,
    Guid UnitId,
    DateOnly ExpirationDate,
    TradeItemFrom From);
