
using System.Security.AccessControl;
using JasperFx.Events;
using Marten.Events.Aggregation;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
#pragma warning disable S125 // Sections of code should not be commented out
namespace Pento.Application.FoodItems.Projections;
public record class FoodItemDetail(
    Guid Id,
    Guid FoodRefId,
    Guid CompartmentId,
    string CompartmentName,
    Guid HouseholdId,
    string Name,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    Guid UnitId,
    DateTime ExpirationDateUtc,
    string? Notes,
    Guid? SourceItemId,
    DateTimeOffset Timestamp,
    string? AddedBy,
    int Version = 1
    
);
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
public sealed class FoodItemDetailProjection : SingleStreamProjection<FoodItemDetail, Guid>
{
    public FoodItemDetailProjection()
    {
    }

    //public FoodItemDetail Create(FoodItemAdded e)
    //    => new(e.Id, 
    //        e.FoodRefId, 
    //        e.CompartmentId, 
    //        e.CompartmentName, 
    //        e.HouseholdId, 
    //        e.Name, 
    //        e.ImageUrl, 
    //        e.Quantity, 
    //        e.UnitAbbreviation, 
    //        e.UnitId, 
    //        e.ExpirationDateUtc, 
    //        e.Notes,
    //        e.SourceItemId);
    public FoodItemDetail Create(IEvent<FoodItemAdded> e)
        => new(e.Id,
            e.Data.FoodRefId,
            e.Data.CompartmentId,
            e.Data.CompartmentName,
            e.Data.HouseholdId,
            e.Data.Name,
            e.Data.ImageUrl,
            e.Data.Quantity,
            e.Data.UnitAbbreviation,
            e.Data.UnitId,
            e.Data.ExpirationDateUtc,
            e.Data.Notes,
            e.Data.SourceItemId,
            e.Timestamp,
            e.UserName);
    public FoodItemDetail Apply(FoodItemRenamed @event, FoodItemDetail item) =>
        item with { Name = @event.NewName };
    public FoodItemDetail Apply(FoodItemImageUpdated @event, FoodItemDetail item) =>
        item with { ImageUrl = @event.ImageUrl };
    public FoodItemDetail Apply(FoodItemNotesUpdated @event, FoodItemDetail item) =>
        item with { Notes = @event.Notes };
    public FoodItemDetail Apply(FoodItemExpirationDateUpdated @event, FoodItemDetail item) =>
        item with { ExpirationDateUtc = @event.ExpirationDateUtc };
    public FoodItemDetail Apply(FoodItemCompartmentMoved @event, FoodItemDetail item) =>
        item with { CompartmentId = @event.CompartmentId };
    public FoodItemDetail Apply(FoodItemQuantityAdjusted @event, FoodItemDetail item) =>
        item with { Quantity = @event.Quantity };
    public FoodItemDetail Apply(FoodItemUnitChanged @event, FoodItemDetail item) =>
        item with { UnitId = @event.UnitId, UnitAbbreviation = @event.UnitAbbreviation, Quantity = @event.ConvertedQuantity };
    public FoodItemDetail Apply(FoodItemReservedForRecipe @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
    public FoodItemDetail Apply(FoodItemReservedForMealPlan @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
    public FoodItemDetail Apply(FoodItemReservedForDonation @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
    public FoodItemDetail Apply(FoodItemReservedForRecipeCancelled @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.Quantity };
    public FoodItemDetail Apply(FoodItemReservedForMealPlanCancelled @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.Quantity };
    public FoodItemDetail Apply(FoodItemReservedForDonationCancelled @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.Quantity };
    public FoodItemDetail Apply(FoodItemReservedForRecipeConsumed @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.ReservedQuantity - @event.ConsumedQuantity };
    public FoodItemDetail Apply(FoodItemReservedForMealPlanConsumed @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.ReservedQuantity - @event.ConsumedQuantity };
    public FoodItemDetail Apply(FoodItemReservedForDonationDonated @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.ReservedQuantity - @event.DonatedQuantity };
    public FoodItemDetail Apply(FoodItemConsumed @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
    public FoodItemDetail Apply(FoodItemDiscarded @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
    public FoodItemDetail Apply(FoodItemSplit @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
    public FoodItemDetail Apply(FoodItemMerged @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + @event.Quantity };
    public FoodItemDetail Apply(FoodItemRemovedByMerge @event, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - @event.Quantity };
}
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
