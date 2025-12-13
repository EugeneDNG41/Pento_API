namespace Pento.Application.Trades.Get;

public sealed record TradePostResponse(
    Guid OfferId,
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    string PostedByName,
    Uri? PostedByAvatarUrl,
    decimal Quantity,
    string UnitAbbreviation,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    string PickupOption,
    Guid PostedBy,
    DateTime CreatedOnUtc
);

public sealed record TradePostGroupedResponse(
    Guid OfferId,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    string PickupOption,
    Guid PostedBy,
    DateTime CreatedOnUtc,
    string PostedByName,
    Uri? PostedByAvatarUrl,
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
