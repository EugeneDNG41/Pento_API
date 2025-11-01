
using System.Security.AccessControl;
using Marten.Events.Aggregation;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.Projections;
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
public sealed class FoodItemProjection : SingleStreamProjection<FoodItem, Guid>
{
    public FoodItemProjection()
    {
    }

    public static FoodItem Create(FoodItemAdded e)
        => new(e.Id, e.FoodRefId, e.CompartmentId, e.HouseholdId, e.CustomName, e.Quantity, e.UnitId, e.ExpirationDateUtc, e.Notes);
    public void Apply(FoodItemRenamed @event, FoodItem item)
    {
        item.CustomName = @event.NewCustomName;
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
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
