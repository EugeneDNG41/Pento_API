using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JasperFx.Events;
using Marten.Events.Aggregation;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;

namespace Pento.Domain.FoodItems.Projections;

public record class FoodItemPreview(
    Guid Id,
    Guid CompartmentId,
    Guid HouseholdId,
    string Name,
    FoodGroup FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime ExpirationDateUtc,
    int Version = 1
);
public sealed class FoodItemPreviewProjection : SingleStreamProjection<FoodItemPreview, Guid>
{
    public FoodItemPreviewProjection()
    {
    }
    public static FoodItemPreview Create(IEvent<FoodItemAdded> e)
        => new(e.Id,
            e.Data.CompartmentId,
            e.Data.HouseholdId,
            e.Data.Name,
            e.Data.FoodGroup,
            e.Data.ImageUrl,
            e.Data.Quantity,
            e.Data.UnitAbbreviation,
            e.Data.ExpirationDateUtc);
    public FoodItemPreview Apply(IEvent<FoodItemRenamed> e, FoodItemPreview item) =>
        item with { Name = e.Data.NewName};
    public FoodItemPreview Apply(IEvent<FoodItemImageUpdated> e, FoodItemPreview item) =>
        item with { ImageUrl = e.Data.ImageUrl };
    public FoodItemPreview Apply(IEvent<FoodItemExpirationDateUpdated> e, FoodItemPreview item) =>
        item with { ExpirationDateUtc = e.Data.ExpirationDateUtc};
    public FoodItemPreview Apply(IEvent<FoodItemCompartmentMoved> e, FoodItemPreview item) =>
        item with { CompartmentId = e.Data.CompartmentId};
    public FoodItemPreview Apply(IEvent<FoodItemQuantityAdjusted> e, FoodItemPreview item) =>
        item with { Quantity = e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemUnitChanged> e, FoodItemPreview item) =>
        item with { UnitAbbreviation = e.Data.UnitAbbreviation, Quantity = e.Data.ConvertedQuantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForRecipe> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForMealPlan> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForDonation> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForRecipeCancelled> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForMealPlanCancelled> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForDonationCancelled> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForRecipeConsumed> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.ConsumedQuantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForMealPlanConsumed> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.ConsumedQuantity};
    public FoodItemPreview Apply(IEvent<FoodItemReservedForDonationDonated> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.DonatedQuantity};
    public FoodItemPreview Apply(IEvent<FoodItemConsumed> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemDiscarded> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity };
    public FoodItemPreview Apply(IEvent<FoodItemSplit> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity};
    public FoodItemPreview Apply(IEvent<FoodItemMerged> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity };
    public FoodItemPreview Apply(IEvent<FoodItemRemovedByMerge> e, FoodItemPreview item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity};
}
