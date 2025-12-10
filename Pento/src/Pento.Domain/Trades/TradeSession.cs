using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeSession : Entity
{
    private TradeSession() { }

    public Guid TradeOfferId { get; private set; }
    public Guid TradeRequestId { get; private set; }
    public Guid OfferUserId { get; private set; }
    public Guid RequestUserId { get; private set; }
    public bool ConfirmedByOfferUser { get; private set; }
    public bool ConfirmedByRequestUser { get; private set; }
    public TradeSessionStatus Status { get; private set; }
    public DateTime StartedOn { get; private set; }

    private TradeSession(
        Guid id,
        Guid tradeOfferId,
        Guid tradeRequestId,
        Guid offerUserId,
        Guid requestUserId,
        TradeSessionStatus status,
        DateTime startedOn
    ) : base(id)
    {
        TradeOfferId = tradeOfferId;
        TradeRequestId = tradeRequestId;
        OfferUserId = offerUserId;
        RequestUserId = requestUserId;
        Status = status;
        StartedOn = startedOn;
    }

    public static TradeSession Create(Guid offerId, Guid requestId, Guid offerUserId,
        Guid requestUserId, DateTime startedOn)
    {
        return new TradeSession(
            id: Guid.CreateVersion7(),
            tradeOfferId: offerId,
            tradeRequestId: requestId,
            offerUserId: offerUserId,
            requestUserId: requestUserId,
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
    public void ConfirmByOfferUser()
    {
        ConfirmedByOfferUser = true;
        if (ConfirmedByRequestUser)
        {
            Complete();
        }
    }
    public void ConfirmByRequestUser()
    {
        ConfirmedByRequestUser = true;
        if (ConfirmedByOfferUser)
        {
            Complete();
        }
    }
}
public sealed class TradeSessionCreatedDomainEvent(Guid tradeSessionId) : DomainEvent
{
    public Guid TradeSessionId { get; } = tradeSessionId;
}
public sealed class TradeSessionCompletedDomainEvent(Guid tradeSessionId) : DomainEvent
{
    public Guid TradeSessionId { get; } = tradeSessionId;
}
public sealed class TradeSessionCancelledDomainEvent(Guid tradeSessionId) : DomainEvent
{
    public Guid TradeSessionId { get; } = tradeSessionId;
}
