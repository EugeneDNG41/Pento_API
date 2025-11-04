using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeMedia;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Users;
using JasperFx.Events;

namespace Pento.Domain.FoodItems;
public sealed class FoodItem
{
    public FoodItem(
        Guid id,
        Guid foodReferenceId, 
        Guid compartmentId,
        Guid householdId,
        string name,
        Uri? imageUrl,
        decimal quantity, 
        Guid unitId, 
        DateTime expirationDateUtc, 
        string? notes,
        Guid? sourceItemId)
    {
        Id = id;
        FoodReferenceId = foodReferenceId;
        CompartmentId = compartmentId;
        HouseholdId = householdId;
        Name = name;
        ImageUrl = imageUrl;
        Quantity = quantity;
        UnitId = unitId;
        ExpirationDateUtc = expirationDateUtc;
        Notes = notes;
        SourceItemId = sourceItemId;
    }
    public FoodItem() { }
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid FoodReferenceId { get; private set; }
    public Guid CompartmentId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public string Name { get; private set; }
    public Uri? ImageUrl { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public DateTime ExpirationDateUtc { get; private set; }
    public string? Notes { get; private set; } 
    public Guid? SourceItemId { get; private set; } // If created from split
    
    public static FoodItem Create(IEvent<FoodItemAdded> @event)
        => new(
            @event.Data.Id,
            @event.Data.FoodRefId,
            @event.Data.CompartmentId,
            @event.Data.HouseholdId,
            @event.Data.Name,
            @event.Data.ImageUrl,
            @event.Data.Quantity,
            @event.Data.UnitId,
            @event.Data.ExpirationDateUtc,
            @event.Data.Notes,
            @event.Data.SourceItemId);
    public void Apply(FoodItemRenamed @event)
        => Name = @event.NewName;
    public void Apply(FoodItemNotesUpdated @event)
        => Notes = @event.Notes;
    public void Apply(FoodItemImageUpdated @event)
        => ImageUrl = @event.ImageUrl;
    public void Apply(FoodItemExpirationDateUpdated @event)
        => ExpirationDateUtc = @event.ExpirationDateUtc;
    public void Apply(FoodItemCompartmentMoved @event)
        => CompartmentId = @event.CompartmentId;
    public void Apply(FoodItemQuantityAdjusted @event)
        => Quantity = @event.Quantity;
    public void Apply(FoodItemUnitChanged @event)
    {
        UnitId = @event.UnitId;
        Quantity = @event.ConvertedQuantity;
    }
    public void Apply(FoodItemReservedForRecipe @event)
        => Quantity -= @event.Quantity;
    public void Apply(FoodItemReservedForMealPlan @event)
        => Quantity -= @event.Quantity;
    public void Apply(FoodItemReservedForDonation @event)
        => Quantity -= @event.Quantity;
    public void Apply(FoodItemReservedForRecipeCancelled @event)
        => Quantity += @event.Quantity;
    public void Apply(FoodItemReservedForMealPlanCancelled @event)
        => Quantity += @event.Quantity;
    public void Apply(FoodItemReservedForDonationCancelled @event)
        => Quantity += @event.Quantity;
    public void Apply(FoodItemReservedForRecipeConsumed @event)
        => Quantity += @event.ReservedQuantity - @event.ConsumedQuantity;
    public void Apply(FoodItemReservedForMealPlanConsumed @event)
        => Quantity += @event.ReservedQuantity - @event.ConsumedQuantity;
    public void Apply(FoodItemReservedForDonationDonated @event)
        => Quantity += @event.ReservedQuantity - @event.DonatedQuantity;
    public void Apply(FoodItemConsumed @event)
        => Quantity -= @event.Quantity;
    public void Apply(FoodItemDiscarded @event)
        => Quantity -= @event.Quantity;
    public void Apply(FoodItemSplit @event)
        => Quantity -= @event.Quantity;
    public void Apply(FoodItemMerged @event)
        => Quantity += @event.Quantity;
    public void Apply(FoodItemRemovedByMerge @event)
        => Quantity -= @event.Quantity;
}
