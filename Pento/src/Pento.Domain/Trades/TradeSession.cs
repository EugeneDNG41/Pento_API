using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeSession : Entity
{
    private TradeSession() { }

    public Guid TradeOfferId { get; private set; }
    public Guid TradeRequestId { get; private set; }
    public Guid OfferHouseholdId { get; private set; }
    public Guid RequestHouseholdId { get; private set; }
    public Guid? ConfirmedByOfferUserId { get; private set; }
    public Guid? ConfirmedByRequestUserId { get; private set; }
    public TradeSessionStatus Status { get; private set; }
    public DateTime StartedOn { get; private set; }

    private TradeSession(
        Guid id,
        Guid tradeOfferId,
        Guid tradeRequestId,
        Guid offerHouseholdId,
        Guid requestHouseholdId,
        TradeSessionStatus status,
        DateTime startedOn
    ) : base(id)
    {
        TradeOfferId = tradeOfferId;
        TradeRequestId = tradeRequestId;
        OfferHouseholdId = offerHouseholdId;
        RequestHouseholdId = requestHouseholdId;
        Status = status;
        StartedOn = startedOn;
    }

    public static TradeSession Create(Guid offerId, Guid requestId, Guid offerHouseholdId,
        Guid requestHouseholdId, DateTime startedOn)
    {
        return new TradeSession(
            id: Guid.CreateVersion7(),
            tradeOfferId: offerId,
            tradeRequestId: requestId,
            offerHouseholdId: offerHouseholdId,
            requestHouseholdId: requestHouseholdId,
            status: TradeSessionStatus.Ongoing,
            startedOn: startedOn
        );
    }

    public void Complete()
    {
        Status = TradeSessionStatus.Completed;
        Raise(new TradeSessionCompletedDomainEvent(Id));
    }
    public void Cancel()
    {
        Status = TradeSessionStatus.Cancelled;
        Raise(new TradeSessionCancelledDomainEvent(Id));
    }
    public void ConfirmByOfferUser(Guid? userId)
    {
        ConfirmedByOfferUserId = userId;
        if (ConfirmedByRequestUserId != null && ConfirmedByOfferUserId != null)
        {
            Complete();
        }
    }
    public void ConfirmByRequestUser(Guid? userId)
    {
        ConfirmedByRequestUserId = userId;
        if (ConfirmedByOfferUserId != null && ConfirmedByRequestUserId != null)
        {
            Complete();
        }
    }
}
public sealed class TradeSessionCreatedDomainEvent(Guid tradeSessionId) : DomainEvent
{
    public Guid TradeSessionId { get; } = tradeSessionId; //not needed currently
}
public sealed class TradeSessionCompletedDomainEvent(Guid tradeSessionId) : DomainEvent
{
    public Guid TradeSessionId { get; } = tradeSessionId;
}
public sealed class TradeSessionCancelledDomainEvent(Guid tradeSessionId) : DomainEvent
{
    public Guid TradeSessionId { get; } = tradeSessionId;
}
