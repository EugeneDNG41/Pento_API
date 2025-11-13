using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;

namespace Pento.Domain.FoodItems.Events;


public class FoodItemAddedDomainEvent(Guid foodItemId, decimal quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}

// Property Changes

public class FoodItemQuantityAdjustedDomainEvent(Guid foodItemId, decimal quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
public record FoodItemUnitChanged(Guid UnitId, decimal ConvertedQuantity);
// Reservations
public class FoodItemReservedDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
}
public class FoodItemReservationCancelledDomainEvent(Guid reservationId) : DomainEvent
{
    public Guid ReservationId { get; } = reservationId;
}
public class FoodItemReservedForMealPlanDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
}
public class FoodItemReservedForMealPlanCancelledDomainEvent(Guid reservationId) : DomainEvent
{
    public Guid ReservationId { get; } = reservationId;
}
public record FoodItemReservedForMealPlanConsumed(decimal ReservedQuantity, decimal ConsumedQuantity, Guid MealPlanId);
public record FoodItemReservedForRecipe(decimal Quantity, Guid RecipeId, DateTime? ReservationExpiresOnUtc);
public record FoodItemReservedForRecipeCancelled(decimal Quantity, Guid RecipeId);
public record FoodItemReservedForRecipeConsumed(decimal ReservedQuantity, decimal ConsumedQuantity, Guid RecipeId);
public record FoodItemReservedForDonation(decimal Quantity, Guid DonationId, DateTime? ReservationExpiresOnUtc);
public class FoodItemReservedForDonationCancelledDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
}
public class FoodItemReservedForDonationDonatedDomainEvent(decimal ReservedQuantity, decimal DonatedQuantity, Guid DonationId, Guid RecipientHouseholdId) : DomainEvent
{
    public decimal ReservedQuantity { get; } = ReservedQuantity;
    public decimal DonatedQuantity { get; } = DonatedQuantity;
    public Guid DonationId { get; } = DonationId;
    public Guid RecipientHouseholdId { get; } = RecipientHouseholdId;
}
public class FoodItemConsumedDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
public class FoodItemDiscardedDomainEvent(Guid foodItemId, decimal Quantity, Guid unitId, Guid userId) : DomainEvent
{
    public Guid FoodItemId { get; } = foodItemId;
    public decimal Quantity { get; } = Quantity;
    public Guid UnitId { get; } = unitId;
    public Guid UserId { get; } = userId;
}
public class FoodItemSplitDomainEvent(decimal Quantity) : DomainEvent
{
    public decimal Quantity { get; } = Quantity;
}
public class FoodItemMergedDomainEvent(Guid SourceItemId, decimal Quantity) : DomainEvent
{
    public Guid SourceItemId { get; } = SourceItemId;
    public decimal Quantity { get; } = Quantity;
}
public class FoodItemRemovedByMergeDomainEvent(Guid TargetItemId, decimal Quantity) : DomainEvent
{
    public Guid TargetItemId { get; } = TargetItemId;
    public decimal Quantity { get; } = Quantity;
}
