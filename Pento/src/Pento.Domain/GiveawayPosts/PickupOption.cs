using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GiveawayPosts;
public enum PickupOption
{
    InPerson =1,
    Delivery =2,
    Flexible =3
}

public abstract class Trade : Entity
{
    public Guid UserId { get; private set; }
    public GiveawayStatus Status { get; private set; }
    public PickupOption PickupOption { get; private set; }
    public TradeType Type { get; private set; }
    public bool AllowPartialFulfillment { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
}

public sealed class TradeWant : Trade
{
    public Guid FoodReferenceId { get; private set; }
    public int Quantity { get; private set; }
    public Guid UnitId { get; private set; }
}

public sealed class TradeGiveaway : Trade
{
    public Guid FoodItemId { get; private set; }
    public int Quantity { get; private set; }
    public Guid UnitId { get; private set; }
}

public abstract class TradeRequest : Entity
{
    public Guid TradeId { get; private set; }
    public Guid UserId { get; private set; }
    public TradeRequestStatus Status { get; private set; }
    public TradeType Type { get; private set; }
    public bool IsPartialFulfillment { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    public void Accept() => Status = TradeRequestStatus.Accepted;
    public void Reject() => Status = TradeRequestStatus.Rejected;
    public void Cancel() => Status = TradeRequestStatus.Cancelled;
}

public sealed class TradeWantRequest : TradeRequest
{
    public Guid FoodItemId { get; private set; }
    public int Quantity { get; private set; }
    public Guid UnitId { get; private set; }
}
public sealed class TradeGiveawayRequest : TradeRequest
{
    public Guid FoodReferenceId { get; private set; }
    public int Quantity { get; private set; }
    public Guid UnitId { get; private set; }
}
public enum TradeRequestStatus { Pending, Accepted, Rejected, Cancelled }
public enum TradeType { Want, Giveaway }
