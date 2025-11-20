using Pento.Application.Users.Search;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;

namespace Pento.Application.FoodItems.GetById;

public sealed record FoodItemDetail(
    Guid Id,
    string FoodReferenceName,
    string StorageName,
    string StorageType,
    string CompartmentName,
    string Name,
    string? Brand,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    string UnitType,
    DateOnly ExpirationDate,
    string Status,
    int TypicalPantryShelfLifeDays,
    int TypicalShelfLifeDays,
    int TypicalFreezerShelfLifeDays,
    string? Notes,
    BasicUserResponse AddedBy,
    BasicUserResponse? LastModifiedBy
);
public sealed record FoodItemDetailRow
{
    public Guid Id { get; init; }
    public string FoodReferenceName { get; init; }
    public string StorageName { get; init; }
    public StorageType StorageType { get; init; }
    public string CompartmentName { get; init; }
    public FoodGroup FoodGroup { get; init; }
    public string Name { get; init; }
    public string? Brand { get; init; }
    public Uri? ImageUrl { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; }
    public string UnitType { get; init; }
    public DateOnly ExpirationDate { get; init; }
    public string Status { get; init; }
    public int TypicalPantryShelfLifeDays { get; init; }
    public int TypicalFridgeShelfLifeDays { get; init; }
    public int TypicalFreezerShelfLifeDays { get; init; }
    public string? Notes { get; init; }
    public Guid AddedById { get; init; }
    public Guid? LastModifiedById { get; init; }

}
