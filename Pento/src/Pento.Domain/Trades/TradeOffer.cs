using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeOffer : Entity
{
    public Guid UserId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public TradeOfferStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PickupOption PickupOption { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    public TradeOffer(
           Guid id,
           Guid userId,
           Guid householdId,
           TradeOfferStatus status,
           DateTime startDate,
           DateTime endDate,
           PickupOption pickupOption,
           DateTime createdOn
       ) : base(id)
    {
        UserId = userId;
        HouseholdId = householdId;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        PickupOption = pickupOption;
        CreatedOn = createdOn;
        UpdatedOn = null;

    }
    public static TradeOffer Create(
        Guid userId,
        Guid householdId,
        DateTime startDate,
        DateTime endDate,
        PickupOption pickupOption,
        DateTime createdOn
        )
    {
        return new TradeOffer(
            id: Guid.CreateVersion7(),
            userId: userId,
            householdId: householdId,
            status: TradeOfferStatus.Open,
            startDate: startDate,
            endDate: endDate,
            pickupOption: pickupOption,
            createdOn: createdOn
        );

    }
    public void Update(DateTime start, DateTime end, PickupOption option, DateTime updatedOn)
    {
        StartDate = start;
        EndDate = end;
        PickupOption = option;
        UpdatedOn = updatedOn;
    }
    public void Cancel()
    {
        Status = TradeOfferStatus.Cancelled;
        Raise(new TradeOfferCancelledDomainEvent(Id));
    }
    public void Expire()
    {
        Status = TradeOfferStatus.Expired;
        Raise(new TradeOfferExpiredDomainEvent(Id));
    }
    public void AutoCancel()
    {
        Status = TradeOfferStatus.Cancelled;
    }
    public void Fulfill(Guid requestId)
    {
        Status = TradeOfferStatus.Fulfilled;
        Raise(new TradeOfferFulfilledDomainEvent(Id, requestId));
    }
    public new void Delete()
    {
        if (Status == TradeOfferStatus.Open)
        {
            Cancel();
        }
        base.Delete();
    }
}
public sealed class TradeOfferCancelledDomainEvent(Guid tradeOfferId) : DomainEvent
{
    public Guid TradeOfferId { get; } = tradeOfferId;
}
public sealed class TradeOfferExpiredDomainEvent(Guid tradeOfferId) : DomainEvent
{
    public Guid TradeOfferId { get; } = tradeOfferId;
}
public sealed class TradeOfferFulfilledDomainEvent(Guid tradeOfferId, Guid requestId) : DomainEvent
{
    public Guid TradeOfferId { get; } = tradeOfferId;
    public Guid TradeRequestId { get; } = requestId;
}
