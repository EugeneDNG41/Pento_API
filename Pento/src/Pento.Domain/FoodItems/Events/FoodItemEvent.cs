using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.FoodItems.Events;

public abstract record FoodItemEvent();
public record FoodItemAdded(
    Guid Id, 
    Guid FoodRefId, 
    Guid CompartmentId,
    Guid HouseholdId,
    string? CustomName, 
    decimal Quantity, 
    Guid UnitId, 
    DateTime ExpirationDateUtc,
    string? Notes,
    Guid? SourceItemId) : FoodItemEvent;

// Property Changes
public record FoodItemRenamed(string? NewCustomName) : FoodItemEvent;
public record FoodItemNotesChanged(string? Notes) : FoodItemEvent;
public record FoodItemExpirationChanged(DateTime ExpirationDateUtc) : FoodItemEvent;
public record FoodItemMoved(Guid CompartmentId) : FoodItemEvent;
public record FoodItemQuantityAdjusted(decimal Quantity) : FoodItemEvent;
public record FoodItemUnitChanged(Guid UnitId, decimal ConvertedQuantity) : FoodItemEvent;
// Reservations
public record FoodItemReservedForMealPlan(decimal Quantity, Guid MealPlanId, DateTime? ReservationExpiresOnUtc) : FoodItemEvent;
public record FoodItemReservedForMealPlanCancelled(decimal Quantity, Guid MealPlanId) : FoodItemEvent;
public record FoodItemReservedForMealPlanConsumed(decimal ReservedQuantity, decimal ConsumedQuantity, Guid MealPlanId) : FoodItemEvent;
public record FoodItemReservedForRecipe(decimal Quantity, Guid RecipeId, DateTime? ReservationExpiresOnUtc) : FoodItemEvent;
public record FoodItemReservedForRecipeCancelled(decimal Quantity, Guid RecipeId) : FoodItemEvent;
public record FoodItemReservedForRecipeConsumed(decimal ReservedQuantity, decimal ConsumedQuantity, Guid RecipeId) : FoodItemEvent;
public record FoodItemReservedForDonation(decimal Quantity, Guid DonationId, DateTime? ReservationExpiresOnUtc) : FoodItemEvent;
public record FoodItemReservedForDonationCancelled(decimal Quantity, Guid DonationId): FoodItemEvent;
public record FoodItemReservedForDonationDonated(decimal ReservedQuantity, decimal DonatedQuantity, Guid DonationId, Guid RecipientHouseholdId): FoodItemEvent;
public record FoodItemConsumed(decimal Quantity): FoodItemEvent;
public record FoodItemDisposed(decimal Quantity, DisposalReason Reason): FoodItemEvent;
public record FoodItemSplit(decimal Quantity): FoodItemEvent;
public record FoodItemMerged(Guid SourceItemId, Guid TargetItemId, decimal Quantity): FoodItemEvent;
public record FoodItemRemovedByMerge(Guid TargetItemId, decimal Quantity): FoodItemEvent;
public enum DisposalReason
{
    Expired, //due to past expiration date
    Spoiled, // due to spoilage before expiration date
    Damaged, // due to damage
    Other
}
