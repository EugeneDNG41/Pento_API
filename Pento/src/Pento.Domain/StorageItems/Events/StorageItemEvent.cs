using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.StorageItems.Events;

public abstract record StorageItemEvent();
public record StorageItemCreated(
    Guid Id, 
    Guid FoodRefId, 
    Guid CompartmentId, 
    string? CustomName, 
    decimal Quantity, 
    Guid UnitId, 
    DateTime ExpirationDateUtc,
    string? Notes) : StorageItemEvent;


// Property Changes
public record StorageItemRenamed(string? NewCustomName) : StorageItemEvent;

public record StorageItemNotesChanged(string? Notes) : StorageItemEvent;

public record StorageItemExpirationChanged(DateTime ExpirationDateUtc) : StorageItemEvent;

public record StorageItemMoved(Guid CompartmentId) : StorageItemEvent;

public record StorageItemQuantityAdjusted(decimal Quantity) : StorageItemEvent;
public record StorageItemUnitChanged(Guid UnitId, decimal ConvertedQuantity) : StorageItemEvent;
// Reservations
public record StorageItemReservedForMealPlan(decimal Quantity, Guid MealPlanId, DateTime? ReservationExpiresOnUtc) : StorageItemEvent;
public record StorageItemReservedForMealPlanCancelled(decimal Quantity, Guid MealPlanId) : StorageItemEvent;
public record StorageItemReservedForMealPlanConsumed(decimal Quantity, Guid MealPlanId) : StorageItemEvent;
public record StorageItemReservedForRecipe(decimal Quantity, Guid RecipeId, DateTime? ReservationExpiresOnUtc) : StorageItemEvent;
public record StorageItemReservedForRecipeCancelled(decimal Quantity, Guid RecipeId) : StorageItemEvent;
public record StorageItemReservedForRecipeConsumed(decimal Quantity, Guid RecipeId) : StorageItemEvent;
public record StorageItemReservedForDonation(decimal Quantity, Guid DonationId, DateTime? ReservationExpiresOnUtc) : StorageItemEvent;
public record StorageItemReservedForDonationCancelled(decimal Quantity, Guid DonationId): StorageItemEvent;
public record StorageItemReservedForDonationDonated(decimal Quantity, Guid DonationId, Guid RecipientHouseholdId): StorageItemEvent;

public record StorageItemConsumed(decimal Quantity): StorageItemEvent;
public record StorageItemDisposed(decimal Quantity, DisposalReason Reason): StorageItemEvent;

public record StorageItemSplit(decimal Quantity): StorageItemEvent;

public record StorageItemCreatedFromSplit(     
     Guid Id,
     Guid SourceStorageItemId,
     Guid FoodRefId,
     Guid CompartmentId,
     decimal Quantity,
     Guid UnitId,
     DateTime ExpirationDateUtc,
     string? Notes
): StorageItemEvent;

public record StorageItemMerged(Guid SourceItemId, Guid TargetItemId, decimal Quantity): StorageItemEvent;
public record StorageItemRemovedByMerge(Guid TargetItemId, decimal Quantity): StorageItemEvent;
public enum DisposalReason
{
    Expired, //due to past expiration date
    Spoiled, // due to spoilage before expiration date
    Damaged, // due to damage
    Other
}
