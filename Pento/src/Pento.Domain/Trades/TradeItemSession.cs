namespace Pento.Domain.Trades;

public sealed class TradeItemSession : TradeItem
{
    private TradeItemSession() { }

    private TradeItemSession(
        Guid id,
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid sessionId,
        TradeItemSessionFrom itemFrom
    ) : base(id, foodItemId, quantity, unitId, TradeItemFrom.Session)
    {
        SessionId = sessionId;
        ItemFrom = itemFrom;
    }

    public Guid SessionId { get; private set; }
    public TradeItemSessionFrom ItemFrom { get; private set; }

    public static TradeItemSession Create(
        Guid foodItemId,
        decimal quantity,
        Guid unitId,
        Guid sessionId,
        TradeItemSessionFrom itemFrom)
    {
        return new TradeItemSession(
            id: Guid.CreateVersion7(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            sessionId: sessionId,
            itemFrom
        );
    }
}
public enum TradeItemSessionFrom
{
    Offer, Request
}

