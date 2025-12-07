namespace Pento.Application.Trades.Get;

public sealed record TradePostResponse(
    Guid OfferId,
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime StartDate,
    DateTime EndDate,
    string PickupOption,
    Guid PostedBy,
    DateTime CreatedOnUtc
);
public sealed record TradePostGroupedResponse(
    Guid OfferId,
    DateTime StartDate,
    DateTime EndDate,
    string PickupOption,
    Guid PostedBy,
    DateTime CreatedOnUtc,
    IReadOnlyList<TradePostItemResponse> Items
);

public sealed record TradePostItemResponse(
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    decimal Quantity,
    string UnitAbbreviation
);
