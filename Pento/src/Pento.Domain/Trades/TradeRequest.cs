using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeRequest : Entity
{
    private TradeRequest() { }

    public Guid UserId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public Guid TradeOfferId { get; private set; }
    public TradeRequestStatus Status { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    private TradeRequest(
        Guid id,
        Guid userId,
        Guid householdId,
        Guid tradeOfferId,
        TradeRequestStatus status,
        DateTime createdOn
    ) : base(id)
    {
        UserId = userId;
        HouseholdId = householdId;
        TradeOfferId = tradeOfferId;
        Status = status;
        CreatedOn = createdOn;
    }

    public static TradeRequest Create(Guid userId, Guid householdId, Guid tradeOfferId, DateTime createdOn)
    {
        var request = new TradeRequest(
            id: Guid.CreateVersion7(),
            userId: userId,
            householdId: householdId,
            tradeOfferId: tradeOfferId,
            status: TradeRequestStatus.Pending,
            createdOn: createdOn
        );
        request.Raise(new TradeRequestCreatedDomainEvent(request.Id));
        return request;
    }

    public void Fulfill(Guid offerId)
    {
        Status = TradeRequestStatus.Fulfilled;
        Raise(new TradeRequestFulfilledDomainEvent(Id, offerId));
    }

    public void Reject()
    {
        Status = TradeRequestStatus.Rejected;
        Raise(new TradeRequestRejectedDomainEvent(Id));
    }

    public void Cancel()
    {
        Status = TradeRequestStatus.Cancelled;
        Raise(new TradeRequestCancelledDomainEvent(Id));
    }
    public void AutoCancel()
    {
        Status = TradeRequestStatus.Cancelled;
        // No domain event raised for auto-cancellation
    }
}
public sealed class TradeRequestCreatedDomainEvent(Guid tradeRequestId) : DomainEvent
{
    public Guid TradeRequestId { get; } = tradeRequestId;
}
public sealed class TradeRequestFulfilledDomainEvent(Guid tradeRequestId, Guid tradeOfferId) : DomainEvent
{
    public Guid TradeRequestId { get; } = tradeRequestId;
    public Guid TradeOfferId { get; } = tradeOfferId;
}
public sealed class TradeRequestRejectedDomainEvent(Guid tradeRequestId) : DomainEvent
{
    public Guid TradeRequestId { get; } = tradeRequestId;
}
public sealed class TradeRequestCancelledDomainEvent(Guid tradeRequestId) : DomainEvent
{
    public Guid TradeRequestId { get; } = tradeRequestId;
}
