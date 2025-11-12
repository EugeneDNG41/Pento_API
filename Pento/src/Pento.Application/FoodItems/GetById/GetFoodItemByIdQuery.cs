
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;

namespace Pento.Application.FoodItems.GetById;

public sealed record GetFoodItemByIdQuery(Guid Id) : IQuery<FoodItemDetail>;
public sealed record FoodItemDetail(
    Guid Id,
    string FoodReferenceName,
    string StorageName,
    string StorageType,
    string CompartmentName,
    string Name,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateOnly ExpirationDate,
    string? Notes,
    BasicUserResponse AddedBy,
    BasicUserResponse? LastModifiedBy
);
public sealed record BasicUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
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
    public Uri? ImageUrl { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; }
    public DateOnly ExpirationDate { get; init; }
    public string? Notes { get; init; }
    public Guid AddedById { get; init; }
    public Guid? LastModifiedById { get; init; }

}

