using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.GetById;

public sealed record TradeItemResponse(
    Guid TradeItemId,
    Guid FoodItemId,
    string Name,
    string OriginalName,
    Uri? ImageUrl,
    string FoodGroup,
    decimal Quantity,
    string UnitAbbreviation,
    Guid UnitId,
    DateOnly ExpirationDate,
    TradeItemFrom From);
