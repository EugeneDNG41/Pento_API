using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodItems.Events;


public sealed class FoodItemAddedDomainEvent(Guid foodItemId, decimal quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}

// Property Changes

public sealed class FoodItemQuantityAdjustedDomainEvent(Guid foodItemId, decimal quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
// Reservations
public sealed class FoodItemReservedDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
public sealed class FoodItemReservationCancelledDomainEvent(Guid reservationId) : DomainEvent
{
    public Guid ReservationId { get; } = reservationId;
}
public sealed class FoodItemReservedForMealPlanDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
}

public sealed class FoodItemConsumedDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
public sealed class FoodItemDiscardedDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
public sealed class FoodItemSplitDomainEvent(decimal Quantity) : DomainEvent
{
    public decimal Quantity { get; } = Quantity;
}
public sealed class FoodItemMergedDomainEvent(Guid SourceItemId, decimal Quantity) : DomainEvent
{
    public Guid SourceItemId { get; } = SourceItemId;
    public decimal Quantity { get; } = Quantity;
}
public sealed class FoodItemRemovedByMergeDomainEvent(Guid TargetItemId, decimal Quantity) : DomainEvent
{
    public Guid TargetItemId { get; } = TargetItemId;
    public decimal Quantity { get; } = Quantity;
}
