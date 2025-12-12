using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeSessionItem : BaseEntity
{
    private TradeSessionItem() { }

    private TradeSessionItem(
        Guid id,
        Guid foodItemId,
        decimal quantity,
        TradeItemFrom from,
        Guid unitId,
        Guid sessionId      
    )
    {
        Id = id;
        FoodItemId = foodItemId;
        Quantity = quantity;
        UnitId = unitId;
        From = from;
        SessionId = sessionId;
    }
    public Guid Id { get; private set; }
    public Guid FoodItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public TradeItemFrom From { get; private set; }
    public Guid SessionId { get; private set; }

    public static TradeSessionItem Create(
        Guid foodItemId,
        decimal quantity,      
        Guid unitId,
        TradeItemFrom from,
        Guid sessionId)
    {
        return new TradeSessionItem(
            id: Guid.CreateVersion7(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            sessionId: sessionId,
            from: from
        );
    }
    public void Update(decimal quantity,  Guid unitId)
    {
        Quantity = quantity;
        UnitId = unitId;
    }
}

