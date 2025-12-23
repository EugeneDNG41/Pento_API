using NetTopologySuite.Geometries;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeOffer : Entity
{
    private TradeOffer() { }
    public Guid UserId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public TradeOfferStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PickupOption PickupOption { get; private set; }
    public Point Location { get; private set; }
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
           Point location,
           DateTime createdOn
       ) : base(id)
    {
        UserId = userId;
        HouseholdId = householdId;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        PickupOption = pickupOption;
        Location = location;
        CreatedOn = createdOn;
        UpdatedOn = null;

    }
    public static TradeOffer Create(
        Guid userId,
        Guid householdId,
        DateTime startDate,
        DateTime endDate,
        PickupOption pickupOption,
        Point location,
        DateTime createdOn
        )
    {
        var offer = new TradeOffer(
            id: Guid.CreateVersion7(),
            userId: userId,
            householdId: householdId,
            status: TradeOfferStatus.Open,
            startDate: startDate,
            endDate: endDate,
            pickupOption: pickupOption,
            location: location,
            createdOn: createdOn
        );
        offer.Raise(new TradeOfferCreatedDomainEvent(offer.Id));
        return offer;

    }
    public void Update(PickupOption? option, Point? location, DateTime updatedOn)
    {
        if (option.HasValue)
        {
            PickupOption = option.Value;
        }
        if (location != null)
        {
            Location = location;
        }
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
public sealed class TradeOfferCreatedDomainEvent(Guid tradeOfferId) : DomainEvent
{
    public Guid TradeOfferId { get; } = tradeOfferId;
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
