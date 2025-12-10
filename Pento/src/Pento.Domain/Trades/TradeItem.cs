using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public abstract class TradeItem : BaseEntity
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
    public void Update(decimal quantity, Guid unitId)
    {
        Quantity = quantity;
        UnitId = unitId;
    }
}

