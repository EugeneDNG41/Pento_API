using Pento.Domain.Abstractions;
using Pento.Domain.GiveawayPosts;

namespace Pento.Domain.Trades;

public sealed class TradeOffer : Entity
{
    public Guid UserId { get; private set; }
    public GiveawayStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PickupOption PickupOption { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
}
public sealed class TradeRequest : Entity
{
    public Guid UserId { get; private set; }
    public Guid TradeOfferId { get; private set; }
    public TradeRequestStatus Status { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    public void Accept() => Status = TradeRequestStatus.Accepted;
    public void Reject() => Status = TradeRequestStatus.Rejected;
    public void Cancel() => Status = TradeRequestStatus.Cancelled;
}
public enum TradeRequestStatus { Pending, Accepted, Rejected, Cancelled }
public sealed class TradeSession : Entity
{
    public Guid TradeOfferId { get; private set; }
    public Guid TradeRequestId { get; private set; }
    public TradeSessionStatus Status { get; private set; }
    public DateTime StartedOn { get; private set; }
}
public enum TradeSessionStatus { Ongoing, Completed, Cancelled }
public sealed class TradeSessionMessage : Entity
{
    public Guid TradeSessionId { get; private set; }
    public Guid SenderUserId { get; private set; }
    public string MessageText { get; private set; }
    public DateTime SentOn { get; private set; }
}

public abstract class TradeItem
{
    public Guid Id { get; private set; }
    public Guid FoodItemId { get; private set; }
    public int Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public TradeItemFrom From { get; private set; }
}
public enum TradeItemFrom { Offer, Request, Session }
public sealed class TradeItemOffer : TradeItem
{
    public Guid OfferId { get; private set; }
}
public sealed class TradeItemRequest : TradeItem
{
    public Guid RequestId { get; private set; }
}
public sealed class TradeItemSession : TradeItem
{
    public Guid SessionId { get; private set; }
}
