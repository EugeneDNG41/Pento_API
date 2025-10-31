using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Images;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Users;

namespace Pento.Domain.FoodItems;
public sealed class FoodItem : Entity
{
    public FoodItem(
        Guid id,
        Guid foodRefId, 
        Guid compartmentId,
        Guid householdId,
        string? customName, 
        decimal quantity, 
        Guid unitId, 
        DateTime expirationDateUtc, 
        string? notes) : base(id)
    {
        FoodRefId = foodRefId;
        CompartmentId = compartmentId;
        HouseholdId = householdId;
        CustomName = customName;
        Quantity = quantity;
        UnitId = unitId;

        ExpirationDateUtc = expirationDateUtc;
        Notes = notes;
    }
    private FoodItem() { }
    public Guid FoodRefId { get; private set; }
    public Guid CompartmentId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public string? CustomName { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public DateTime ExpirationDateUtc { get; private set; }
    public string? Notes { get; private set; }
    
    public Guid? SourceItemId { get; private set; } // If created from split
    public void Apply(FoodItemCreated @event) 
    {
        FoodRefId = @event.FoodRefId;
        CompartmentId = @event.CompartmentId;
        HouseholdId = @event.HouseholdId;
        CustomName = @event.CustomName;
        Quantity = @event.Quantity;
        UnitId = @event.UnitId;
        ExpirationDateUtc = @event.ExpirationDateUtc;
        Notes = @event.Notes;
    }

    public void Apply(FoodItemRenamed @event)
    {
        CustomName = @event.NewCustomName;
    }

    public void Apply(FoodItemNotesChanged @event)
    {
        Notes = @event.Notes;
    }

    public void Apply(FoodItemExpirationChanged @event)
    {
        ExpirationDateUtc = @event.ExpirationDateUtc;
    }

    public void Apply(FoodItemMoved @event)
    {
        CompartmentId = @event.CompartmentId;
    }

    public void Apply(FoodItemQuantityAdjusted @event)
    {
        Quantity = @event.Quantity;
    }

    public void Apply(FoodItemUnitChanged @event)
    {
        UnitId = @event.UnitId;
        Quantity = @event.ConvertedQuantity;
    }

    public void Apply(FoodItemReservedForRecipe @event)
    {
        Quantity -= @event.Quantity;
    }
    public void Apply(FoodItemReservedForMealPlan @event)
    {
        Quantity -= @event.Quantity;
    }
    public void Apply(FoodItemReservedForDonation @event)
    {
        Quantity -= @event.Quantity;
    }
    public void Apply(FoodItemReservedForRecipeCancelled @event)
    {
        Quantity += @event.Quantity;
    }
    public void Apply(FoodItemReservedForMealPlanCancelled @event)
    {
        Quantity += @event.Quantity;
    }
    public void Apply(FoodItemReservedForDonationCancelled @event)
    {
        Quantity += @event.Quantity;
    }
    public void Apply(FoodItemReservedForRecipeConsumed @event)
    {
        Quantity += @event.Quantity;
    }

    public void Apply(FoodItemConsumed @event)
    {
        Quantity -= @event.Quantity;
    }
    public void Apply(FoodItemDisposed @event)
    {
        Quantity -= @event.Quantity;
    }

    public void Apply(FoodItemSplit @event)
    {
        Quantity -= @event.Quantity;
    }
    public void Apply(FoodItemCreatedFromSplit @event)
    {
        SourceItemId = @event.SourceFoodItemId;
        FoodRefId = @event.FoodRefId;
        CompartmentId = @event.CompartmentId;
        Quantity = @event.Quantity;
        UnitId = @event.UnitId;
        ExpirationDateUtc = @event.ExpirationDateUtc;
        Notes = @event.Notes;
    }
    public void Apply(FoodItemMerged @event)
    {
        Quantity += @event.Quantity;
    }
    public void Apply(FoodItemRemovedByMerge @event)
    {
        Quantity -= @event.Quantity;
    }
}
