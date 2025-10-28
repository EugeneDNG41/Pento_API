using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.StorageItems.Events;

namespace Pento.Domain.StorageItems;
public sealed class StorageItem : Entity
{
    public StorageItem(
        Guid id,
        Guid foodRefId, 
        Guid compartmentId, 
        string? customName, 
        decimal quantity, 
        Guid unitId, 
        DateTime expirationDateUtc, 
        string? notes) : base(id)
    {
        FoodRefId = foodRefId;
        CompartmentId = compartmentId;
        CustomName = customName;
        Quantity = quantity;
        UnitId = unitId;

        ExpirationDateUtc = expirationDateUtc;
        Notes = notes;
    }
    private StorageItem() { }
    public Guid FoodRefId { get; private set; }
    public Guid CompartmentId { get; private set; }
    public string? CustomName { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public DateTime ExpirationDateUtc { get; private set; }
    public string? Notes { get; private set; }
    public Guid? SourceItemId { get; private set; } // If created from split
    public void Apply(StorageItemCreated @event) 
    {
        FoodRefId = @event.FoodRefId;
        CompartmentId = @event.CompartmentId;
        CustomName = @event.CustomName;
        Quantity = @event.Quantity;
        UnitId = @event.UnitId;
        ExpirationDateUtc = @event.ExpirationDateUtc;
        Notes = @event.Notes;
    }

    public void Apply(StorageItemRenamed @event)
    {
        CustomName = @event.NewCustomName;
    }

    public void Apply(StorageItemNotesChanged @event)
    {
        Notes = @event.Notes;
    }

    public void Apply(StorageItemExpirationChanged @event)
    {
        ExpirationDateUtc = @event.ExpirationDateUtc;
    }

    public void Apply(StorageItemMoved @event)
    {
        CompartmentId = @event.CompartmentId;
    }

    public void Apply(StorageItemQuantityAdjusted @event)
    {
        Quantity = @event.Quantity;
    }

    public void Apply(StorageItemUnitChanged @event)
    {
        UnitId = @event.UnitId;
        Quantity = @event.ConvertedQuantity;
    }

    public void Apply(StorageItemReserved @event)
    {
        Quantity -= @event.Quantity;
    }

    public void Apply(StorageItemReservationCancelled @event)
    {
        Quantity += @event.Quantity;
    }

    public void Apply(StorageItemConsumed @event)
    {
        Quantity -= @event.Quantity;
    }

    public void Apply(StorageItemDonated @event)
    {
        Quantity -= @event.Quantity;
    }

    public void Apply(StorageItemDisposed @event)
    {
        Quantity -= @event.Quantity;
    }


    public void Apply(StorageItemSplit @event)
    {
        Quantity -= @event.Quantity;
    }

    public void Apply(StorageItemCreatedFromSplit @event)
    {
        SourceItemId = @event.SourceStorageItemId;
        FoodRefId = @event.FoodRefId;
        CompartmentId = @event.CompartmentId;
        Quantity = @event.Quantity;
        UnitId = @event.UnitId;
        ExpirationDateUtc = @event.ExpirationDateUtc;
        Notes = @event.Notes;
    }

    public void Apply(StorageItemMerged @event)
    {
        Quantity += @event.Quantity;
    }
    public void Apply(StorageItemRemovedByMerge @event)
    {
        Quantity -= @event.Quantity;
    }
}
