namespace Pento.Domain.Trades;

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
            id: Guid.CreateVersion7(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            requestId: requestId
        );
    }
    
}

