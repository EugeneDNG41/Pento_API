using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.FoodReferences;

namespace Pento.Domain.FoodItems.Events;

public abstract record FoodItemEvent();
public record FoodItemAdded(
    Guid Id, 
    Guid FoodReferenceId,
    string FoodReferenceName,
    Guid CompartmentId,
    string CompartmentName,
    Guid HouseholdId,
    string Name,
    FoodGroup FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    Guid UnitId, 
    DateTime ExpirationDateUtc,
    string? Notes,
    Guid? SourceItemId) : FoodItemEvent;

// Property Changes
public record FoodItemRenamed(string NewName) : FoodItemEvent;
public record FoodItemImageUpdated(Uri? ImageUrl) : FoodItemEvent;
public record FoodItemNotesUpdated(string? Notes) : FoodItemEvent;
public record FoodItemExpirationDateUpdated(DateTime ExpirationDateUtc) : FoodItemEvent;
public record FoodItemCompartmentMoved(Guid CompartmentId, string CompartmentName) : FoodItemEvent;
public record FoodItemQuantityAdjusted(decimal Quantity) : FoodItemEvent;
public record FoodItemUnitChanged(Guid UnitId, decimal ConvertedQuantity, string UnitAbbreviation) : FoodItemEvent;
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
public record FoodItemDiscarded(decimal Quantity, DiscardReason Reason): FoodItemEvent;
public record FoodItemSplit(decimal Quantity): FoodItemEvent;
public record FoodItemMerged(Guid SourceItemId, decimal Quantity): FoodItemEvent;
public record FoodItemRemovedByMerge(Guid TargetItemId, decimal Quantity): FoodItemEvent;
public enum DiscardReason
{
    Expired, //due to past expiration date
    Spoiled, // due to spoilage before expiration date
    Damaged, // due to damage
    Other
}
