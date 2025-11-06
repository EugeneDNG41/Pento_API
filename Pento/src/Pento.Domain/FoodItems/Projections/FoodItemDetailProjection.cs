
using JasperFx.Events;
using Marten.Events.Aggregation;
using Pento.Domain.FoodItems.Events;

namespace Pento.Domain.FoodItems.Projections;
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
    DateTimeOffset AddedAt,
    DateTimeOffset? LastModifiedAt,
    string? AddedBy,
    string? LastModifiedBy,
    int Version = 1   
);
public record class FoodItemPreview(
    Guid Id,
    Guid CompartmentId,
    Guid HouseholdId,
    string Name,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime ExpirationDateUtc,
    int Version = 1
);

public sealed class FoodItemDetailProjection : SingleStreamProjection<FoodItemDetail, Guid>
{
    public FoodItemDetailProjection()
    {
    }
    public static FoodItemDetail Create(IEvent<FoodItemAdded> e)
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
            e.Timestamp,
            e.UserName,
            e.UserName);
    public FoodItemDetail Apply(IEvent<FoodItemRenamed> e, FoodItemDetail item) =>
        item with { Name = e.Data.NewName, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName};
    public FoodItemDetail Apply(IEvent<FoodItemImageUpdated> e, FoodItemDetail item) =>
        item with { ImageUrl = e.Data.ImageUrl, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemNotesUpdated> e, FoodItemDetail item) =>
        item with { Notes = e.Data.Notes, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemExpirationDateUpdated> e, FoodItemDetail item) =>
        item with { ExpirationDateUtc = e.Data.ExpirationDateUtc, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemCompartmentMoved> e, FoodItemDetail item) =>
        item with { CompartmentId = e.Data.CompartmentId, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemQuantityAdjusted> e, FoodItemDetail item) =>
        item with { Quantity = e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemUnitChanged> e, FoodItemDetail item) =>
        item with { UnitId = e.Data.UnitId, UnitAbbreviation = e.Data.UnitAbbreviation, Quantity = e.Data.ConvertedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForRecipe> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForMealPlan> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForDonation> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForRecipeCancelled> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForMealPlanCancelled> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForDonationCancelled> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForRecipeConsumed> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.ConsumedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForMealPlanConsumed> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.ConsumedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemReservedForDonationDonated> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.DonatedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemConsumed> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemDiscarded> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemSplit> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemMerged> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
    public FoodItemDetail Apply(IEvent<FoodItemRemovedByMerge> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = e.UserName };
}
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
