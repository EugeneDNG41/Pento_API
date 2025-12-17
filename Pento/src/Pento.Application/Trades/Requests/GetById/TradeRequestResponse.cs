namespace Pento.Application.Trades.Requests.GetById;

public sealed record TradeRequestResponse
{
    public Guid TradeRequestId {  get; init; }
    public Guid TradeOfferId { get; init; }
    public string OfferHouseholdName { get; init; }
    public string RequestHouseholdName { get; init; }
    public string Status { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public int TotalItems { get; init; }
}
