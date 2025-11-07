
using JasperFx.Events;
using Marten.Events.Aggregation;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.FoodItems.Projections;
public sealed record FoodItemDetail(
    Guid Id,
    string FoodReferenceName,
    string StorageName,
    StorageType StorageType,
    string CompartmentName,
    string Name,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime ExpirationDateUtc,
    string? Notes,
    Guid? SourceItemId,
    DateTimeOffset AddedAt,
    DateTimeOffset? LastModifiedAt,
    BasicUserResponse? AddedBy,
    BasicUserResponse? LastModifiedBy,
    int Version = 1   
);
public sealed record BasicUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
);

public sealed class FoodItemDetailProjection(
    IGenericRepository<User> userRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<Unit> unitRepository) : SingleStreamProjection<FoodItemDetail, Guid>
{
    private async Task<BasicUserResponse?> MapUserToBasicResponseAsync(IEvent e)
    {
        if (e.UserName is not null)
        {
            User user = await userRepository.GetByIdAsync(Guid.Parse(e.UserName));
            return new BasicUserResponse(
                Id: user!.Id,
                FirstName: user!.FirstName,
                LastName: user!.LastName,
                AvatarUrl: user!.AvatarUrl
            );
        }
        return null;
    }
    public async Task<FoodItemDetail> Create(IEvent<FoodItemAdded> e)
    {
        BasicUserResponse? addedBy = await MapUserToBasicResponseAsync(e);
        FoodReference foodReference = await foodReferenceRepository.GetByIdAsync(e.Data.FoodReferenceId);
        Compartment compartment = await compartmentRepository.GetByIdAsync(e.Data.CompartmentId);
        Storage storage = await storageRepository.GetByIdAsync(compartment!.StorageId);
        Unit unit = await unitRepository.GetByIdAsync(e.Data.UnitId);
        return new FoodItemDetail(
            Id: e.Data.Id,
            FoodReferenceName: foodReference!.Name,
            StorageName: storage!.Name,
            StorageType: storage!.Type,
            CompartmentName: compartment!.Name,
            Name: e.Data.Name,
            FoodGroup: foodReference!.FoodGroup.ToReadableString(),
            ImageUrl: e.Data.ImageUrl,
            Quantity: e.Data.Quantity,
            UnitAbbreviation: unit!.Abbreviation,
            ExpirationDateUtc: e.Data.ExpirationDateUtc,
            Notes: e.Data.Notes,
            SourceItemId: e.Data.SourceItemId,
            AddedAt: e.Timestamp,
            LastModifiedAt: null,
            AddedBy: addedBy,
            LastModifiedBy: null
        );
    }

    public async Task<FoodItemDetail> Apply(IEvent<FoodItemRenamed> e, FoodItemDetail item) =>
        item with { Name = e.Data.NewName, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemImageUpdated> e, FoodItemDetail item) =>
        item with { ImageUrl = e.Data.ImageUrl, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemNotesUpdated> e, FoodItemDetail item) =>
        item with { Notes = e.Data.Notes, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemExpirationDateUpdated> e, FoodItemDetail item) =>
        item with { ExpirationDateUtc = e.Data.ExpirationDateUtc, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemCompartmentMoved> e, FoodItemDetail item)
    {
        Compartment compartment = await compartmentRepository.GetByIdAsync(e.Data.CompartmentId);
        return item with { CompartmentName = compartment!.Name, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };      
    }
        
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemQuantityAdjusted> e, FoodItemDetail item) =>
        item with { Quantity = e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemUnitChanged> e, FoodItemDetail item)
    {
        Unit unit = await unitRepository.GetByIdAsync(e.Data.UnitId);
        return item with { UnitAbbreviation = unit!.Abbreviation, Quantity = e.Data.ConvertedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    }
        
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForRecipe> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForMealPlan> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForDonation> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForRecipeCancelled> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForMealPlanCancelled> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForDonationCancelled> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForRecipeConsumed> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.ConsumedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForMealPlanConsumed> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.ConsumedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemReservedForDonationDonated> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.ReservedQuantity - e.Data.DonatedQuantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemConsumed> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemDiscarded> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemSplit> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemMerged> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity + e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
    public async Task<FoodItemDetail> Apply(IEvent<FoodItemRemovedByMerge> e, FoodItemDetail item) =>
        item with { Quantity = item.Quantity - e.Data.Quantity, LastModifiedAt = e.Timestamp, LastModifiedBy = await MapUserToBasicResponseAsync(e) };
}
