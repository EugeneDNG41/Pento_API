
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

    public FoodItem Create(FoodItemAdded e)
        => new(e.Id, e.FoodRefId, e.CompartmentId, e.HouseholdId, e.CustomName, e.Quantity, e.UnitId, e.ExpirationDateUtc, e.Notes, e.SourceItemId);
    public void Apply(FoodItemRenamed @event, FoodItem item)
        => item.Rename(@event.NewCustomName);
    public void Apply(FoodItemNotesChanged @event, FoodItem item)
        => item.UpdateNotes(@event.Notes);
    public void Apply(FoodItemExpirationChanged @event, FoodItem item)
        => item.ChangeExpirationDate(@event.ExpirationDateUtc);
    public void Apply(FoodItemMoved @event, FoodItem item)
        => item.Move(@event.CompartmentId);
    public void Apply(FoodItemQuantityAdjusted @event, FoodItem item)
        => item.AdjustQuantity(@event.Quantity);
    public void Apply(FoodItemUnitChanged @event, FoodItem item)
        => item.ChangeMeasurementUnit(@event.UnitId, @event.ConvertedQuantity);
    public void Apply(FoodItemReservedForRecipe @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
    public void Apply(FoodItemReservedForMealPlan @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
    public void Apply(FoodItemReservedForDonation @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
    public void Apply(FoodItemReservedForRecipeCancelled @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.Quantity);
    public void Apply(FoodItemReservedForMealPlanCancelled @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.Quantity);
    public void Apply(FoodItemReservedForDonationCancelled @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.Quantity);
    public void Apply(FoodItemReservedForRecipeConsumed @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.ReservedQuantity - @event.ConsumedQuantity);
    public void Apply(FoodItemReservedForMealPlanConsumed @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.ReservedQuantity - @event.ConsumedQuantity);
    public void Apply(FoodItemReservedForDonationDonated @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.ReservedQuantity - @event.DonatedQuantity);
    public void Apply(FoodItemConsumed @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
    public void Apply(FoodItemDisposed @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
    public void Apply(FoodItemSplit @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
    public void Apply(FoodItemMerged @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity + @event.Quantity);
    public void Apply(FoodItemRemovedByMerge @event, FoodItem item)
        => item.AdjustQuantity(item.Quantity - @event.Quantity);
}
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
