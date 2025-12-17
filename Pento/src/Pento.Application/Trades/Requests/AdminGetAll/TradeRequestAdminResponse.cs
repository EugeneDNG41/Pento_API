namespace Pento.Application.Trades.Requests.AdminGetAll;

public sealed record TradeRequestAdminResponse
{
    public Guid TradeRequestId { get; init; }
    public Guid TradeOfferId { get; init; }
    public string OfferHouseholdName { get; init; }
    public string RequestHouseholdName { get; init; }
    public string Status { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public int TotalItems { get; init; }
    public bool IsDeleted { get; init; }
}
