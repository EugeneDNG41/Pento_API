namespace Pento.Application.Trades.Offers.GetAll;

public sealed record TradeOfferResponse
{
    public Guid OfferId { get; init; }
    public Guid ItemId { get; init; }
    public Guid FoodItemId { get; init; }
    public string FoodName { get; init; }
    public Uri? FoodImageUri { get; init; }
    public string PostedByName { get; init; } 
    public Uri? PostedByAvatarUrl { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; } 
    public string Status { get; init; } 
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string PickupOption { get; init; } 
    public Guid PostedBy { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public int PendingRequests { get; init; }
}

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
