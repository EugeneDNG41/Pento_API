using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.StorageItems.Events;

public abstract record StorageItemEvent(Guid Id);
public record StorageItemCreated(
    Guid Id, 
    Guid FoodRefId, 
    Guid CompartmentId, 
    string? CustomName, 
    decimal Quantity, 
    Guid UnitId, 
    DateTime ExpirationDateUtc,
    string? Notes) : StorageItemEvent(Id);


// Property Changes
public record StorageItemRenamed
{
    public Guid Id { get; init; }
    public string? NewCustomName { get; init; }
}

public record StorageItemNotesChanged
{
    public Guid Id { get; init; }
    public string? Notes { get; init; }
}

public record StorageItemExpirationChanged
{
    public Guid Id { get; init; }
    public DateTime ExpirationDateUtc { get; init; }
}

public record StorageItemMoved
{
    public Guid Id { get; init; }
    public Guid CompartmentId { get; init; }
}

public record StorageItemQuantityAdjusted
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
}

public record StorageItemUnitChanged
{
    public Guid Id { get; init; }
    public Guid UnitId { get; init; }
    public decimal ConvertedQuantity { get; init; }
}

// Reservations
public record StorageItemReserved
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
    public Guid ForId { get; init; }
    public ReservedForType ForType { get; init; }

    public DateTime? ReservationExpiresOnUtc { get; init; }
}

public record StorageItemReservationCancelled
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
    public Guid ForId { get; init; }
    public ReservedForType ForType { get; init; }
    public string? Reason { get; init; }
}

// Status Changes
public record StorageItemConsumed
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
    public Guid? RecipeId { get; init; }
    public Guid? MealPlanId { get; init; }
}

public record StorageItemDonated
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
    public Guid DonationId { get; init; }
    public Guid RecipientHouseholdId { get; init; }
}

public record StorageItemDisposed
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
    public DisposalReason Reason { get; init; }
}


// Split & Merge Operations
public record StorageItemSplit
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
}

public record StorageItemCreatedFromSplit
{
    public Guid Id { get; init; }
    public Guid SourceStorageItemId { get; init; }
    public Guid FoodRefId { get; init; }
    public Guid CompartmentId { get; init; }
    public decimal Quantity { get; init; }
    public Guid UnitId { get; init; }
    public DateTime ExpirationDateUtc { get; init; }
    public string? Notes { get; init; }
}

public record StorageItemMerged
{
    public Guid Id { get; init; }
    public Guid SourceItemId { get; init; }
    public decimal Quantity { get; init; }
}
public record StorageItemRemovedByMerge
{
    public Guid Id { get; init; }
    public Guid TargetItemId { get; init; }
    public decimal Quantity { get; init; }
}

public enum DisposalReason
{
    Expired,
    Spoiled,
    Damaged,
    Other
}
