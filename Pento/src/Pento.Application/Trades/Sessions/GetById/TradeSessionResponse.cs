using Pento.Application.Users.GetAll;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.GetById;

public sealed record TradeSessionResponse(
    Guid TradeSessionId,
    Guid TradeOfferId,
    Guid TradeRequestId,
    Guid OfferHouseholdId,
    string OfferHouseholdName,
    Guid RequestHouseholdId,
    string RequestHouseholdName,
    TradeSessionStatus Status,
    DateTime StartedOn,
    int TotalOfferedItems,
    int TotalRequestedItems,
    BasicUserResponse? ConfirmedByOfferUser,
    BasicUserResponse? ConfirmedByRequestUser
    );
public sealed record TradeSessionRow
{
    public Guid TradeSessionId { get; init; }
    public Guid TradeOfferId { get; init; }
    public Guid TradeRequestId { get; init; }
    public Guid OfferHouseholdId { get; init; }
    public string OfferHouseholdName { get; init; }
    public Guid RequestHouseholdId { get; init; }
    public string RequestHouseholdName { get; init; }
    public TradeSessionStatus Status { get; init; }
    public DateTime StartedOn { get; init; }
    public int TotalOfferedItems { get; init; }
    public int TotalRequestedItems { get; init; }
    public Guid? ConfirmedByOfferUserId { get; init; }
    public Guid? ConfirmedByRequestUserId { get; init; }
}
