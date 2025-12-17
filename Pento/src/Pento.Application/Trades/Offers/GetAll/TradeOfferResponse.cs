namespace Pento.Application.Trades.Offers.GetAll;

public sealed record TradeOfferResponse(
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
    DateTime CreatedOnUtc,
    int PendingRequests
);

public sealed record TradeOfferGroupedResponse(
    Guid OfferId,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    string PickupOption,
    Guid PostedBy,
    DateTime CreatedOnUtc,
    int PendingRequests,
    string PostedByName,
    Uri? PostedByAvatarUrl,
    IReadOnlyList<TradeOfferItemResponse> Items
);

public sealed record TradeOfferItemResponse(
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    decimal Quantity,
    string UnitAbbreviation
);
