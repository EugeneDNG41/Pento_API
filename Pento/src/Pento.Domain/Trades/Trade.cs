using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeOffer : Entity
{
    public Guid UserId { get; private set; }
    public TradeStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PickupOption PickupOption { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    public TradeOffer(
           Guid id,
           Guid userId,
           TradeStatus status,
           DateTime startDate,
           DateTime endDate,
           PickupOption pickupOption,
           DateTime createdOn
       ) : base(id)
    {
        UserId = userId;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        PickupOption = pickupOption;
        CreatedOn = createdOn;
        UpdatedOn = null;

    }
    public static TradeOffer Create(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        PickupOption pickupOption,
        DateTime createOn
        )
    {
        return new TradeOffer(
            id: Guid.NewGuid(),
            userId: userId,
            status: TradeStatus.Open,
            startDate: startDate,
            endDate: endDate,
            pickupOption: pickupOption,
            createdOn: createOn
        );

    }
    public void Update(DateTime start, DateTime end, PickupOption option)
    {
        StartDate = start;
        EndDate = end;
        PickupOption = option;
        UpdatedOn = DateTime.UtcNow;
    }
}
public sealed class TradeRequest : Entity
{
    private TradeRequest() { }

    public Guid UserId { get; private set; }
    public Guid TradeOfferId { get; private set; }
    public TradeRequestStatus Status { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    private TradeRequest(
        Guid id,
        Guid userId,
        Guid tradeOfferId,
        TradeRequestStatus status,
        DateTime createdOn
    ) : base(id)
    {
        UserId = userId;
        TradeOfferId = tradeOfferId;
        Status = status;
        CreatedOn = createdOn;
    }

    public static TradeRequest Create(Guid userId, Guid tradeOfferId)
    {
        return new TradeRequest(
            id: Guid.NewGuid(),
            userId: userId,
            tradeOfferId: tradeOfferId,
            status: TradeRequestStatus.Pending,
            createdOn: DateTime.UtcNow
        );
    }

    public void Accept()
    {
        Status = TradeRequestStatus.Accepted;
    }

    public void Reject()
    {
        Status = TradeRequestStatus.Rejected;
    }

    public void Cancel()
    {
        Status = TradeRequestStatus.Cancelled;
    }
}

public enum TradeRequestStatus { Pending, Accepted, Rejected, Cancelled }
public sealed class TradeSession : Entity
{
    private TradeSession() { }

    public Guid TradeOfferId { get; private set; }
    public Guid TradeRequestId { get; private set; }
    public TradeSessionStatus Status { get; private set; }
    public DateTime StartedOn { get; private set; }

    private TradeSession(
        Guid id,
        Guid tradeOfferId,
        Guid tradeRequestId,
        TradeSessionStatus status,
        DateTime startedOn
    ) : base(id)
    {
        TradeOfferId = tradeOfferId;
        TradeRequestId = tradeRequestId;
        Status = status;
        StartedOn = startedOn;
    }

    public static TradeSession Create(Guid offerId, Guid requestId)
    {
        return new TradeSession(
            id: Guid.NewGuid(),
            tradeOfferId: offerId,
            tradeRequestId: requestId,
            status: TradeSessionStatus.Ongoing,
            startedOn: DateTime.UtcNow
        );
    }

    public void Complete() => Status = TradeSessionStatus.Completed;
    public void Cancel() => Status = TradeSessionStatus.Cancelled;
}
public enum TradeSessionStatus { Ongoing, Completed, Cancelled }
public sealed class TradeSessionMessage : Entity
{
    private TradeSessionMessage() { }

    public Guid TradeSessionId { get; private set; }
    public Guid SenderUserId { get; private set; }
    public string MessageText { get; private set; }
    public DateTime SentOn { get; private set; }

    private TradeSessionMessage(
        Guid id,
        Guid tradeSessionId,
        Guid senderUserId,
        string messageText,
        DateTime sentOn
    ) : base(id)
    {
        TradeSessionId = tradeSessionId;
        SenderUserId = senderUserId;
        MessageText = messageText;
        SentOn = sentOn;
    }

    public static TradeSessionMessage Create(Guid sessionId, Guid senderUserId, string message)
    {
        return new TradeSessionMessage(
            id: Guid.NewGuid(),
            tradeSessionId: sessionId,
            senderUserId: senderUserId,
            messageText: message,
            sentOn: DateTime.UtcNow
        );
    }
}
public abstract class TradeItem
{
    protected TradeItem() { }

    protected TradeItem(
        Guid id,
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        TradeItemFrom from)
    {
        Id = id;
        FoodItemId = foodItemId;
        Quantity = quantity;
        UnitId = unitId;
        From = from;
    }

    public Guid Id { get; private set; }
    public Guid FoodItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public TradeItemFrom From { get; private set; }
}

public enum TradeItemFrom { Offer, Request, Session }
public sealed class TradeItemOffer : TradeItem
{
    private TradeItemOffer() { }

    private TradeItemOffer(
        Guid id,
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid offerId
    ) : base(id, foodItemId, quantity, unitId, TradeItemFrom.Offer)
    {
        OfferId = offerId;
    }

    public Guid OfferId { get; private set; }

    public static TradeItemOffer Create(
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid offerId)
    {
        return new TradeItemOffer(
            id: Guid.NewGuid(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            offerId: offerId
        );
    }
}

public sealed class TradeItemRequest : TradeItem
{
    private TradeItemRequest() { }

    private TradeItemRequest(
        Guid id,
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid requestId
    ) : base(id, foodItemId, quantity, unitId, TradeItemFrom.Request)
    {
        RequestId = requestId;
    }

    public Guid RequestId { get; private set; }

    public static TradeItemRequest Create(
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid requestId)
    {
        return new TradeItemRequest(
            id: Guid.NewGuid(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            requestId: requestId
        );
    }
}

public sealed class TradeItemSession : TradeItem
{
    private TradeItemSession() { }

    private TradeItemSession(
        Guid id,
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid sessionId
    ) : base(id, foodItemId, quantity, unitId, TradeItemFrom.Session)
    {
        SessionId = sessionId;
    }

    public Guid SessionId { get; private set; }

    public static TradeItemSession Create(
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid sessionId)
    {
        return new TradeItemSession(
            id: Guid.NewGuid(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            sessionId: sessionId
        );
    }
}

