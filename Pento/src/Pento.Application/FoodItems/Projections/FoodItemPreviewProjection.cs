using JasperFx.Events;
using Marten.Events.Aggregation;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Projections;

public record class FoodItemPreview(
    Guid Id,
    string Name,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime ExpirationDateUtc,
    int Version = 1
);
public sealed class FoodItemPreviewProjection(
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository) : SingleStreamProjection<FoodItemPreview, Guid>
{
    public async Task<FoodItemPreview> Create(IEvent<FoodItemAdded> e)
    {
        FoodReference foodReference = await foodReferenceRepository.GetByIdAsync(e.Data.FoodReferenceId);
        Unit unit = await unitRepository.GetByIdAsync(e.Data.UnitId);
        return new FoodItemPreview(
            Id: e.Data.Id,
            Name: e.Data.Name,
            FoodGroup: foodReference!.FoodGroup.ToReadableString(),
            ImageUrl: e.Data.ImageUrl,
            Quantity: e.Data.Quantity,
            UnitAbbreviation: unit!.Abbreviation,
            ExpirationDateUtc: e.Data.ExpirationDateUtc,
            Version: 1
        );
    }
    public FoodItemPreview Apply(IEvent<FoodItemRenamed> e, FoodItemPreview item) =>
        item with { Name = e.Data.NewName};
    public FoodItemPreview Apply(IEvent<FoodItemImageUpdated> e, FoodItemPreview item) =>
        item with { ImageUrl = e.Data.ImageUrl };
    public FoodItemPreview Apply(IEvent<FoodItemExpirationDateUpdated> e, FoodItemPreview item) =>
        item with { ExpirationDateUtc = e.Data.ExpirationDateUtc};
    public FoodItemPreview Apply(IEvent<FoodItemQuantityAdjusted> e, FoodItemPreview item) =>
        item with { Quantity = e.Data.Quantity};
    public async Task<FoodItemPreview> Apply(IEvent<FoodItemUnitChanged> e, FoodItemPreview item)
    {
        Unit unit = await unitRepository.GetByIdAsync(e.Data.UnitId);
        return item with { UnitAbbreviation = unit!.Abbreviation, Quantity = e.Data.ConvertedQuantity };
    }
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
