namespace Pento.Domain.Trades;

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
            id: Guid.CreateVersion7(),
            foodItemId: foodItemId,
            quantity: quantity,
            unitId: unitId,
            offerId: offerId
        );
    }
}

