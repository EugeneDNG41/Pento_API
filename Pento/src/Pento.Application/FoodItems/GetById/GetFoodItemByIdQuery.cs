
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
public sealed record FoodItemDetailRow(
    Guid Id,
    string FoodReferenceName,
    string StorageName,
    StorageType StorageType,
    string CompartmentName,
    string Name,
    FoodGroup FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateOnly ExpirationDate,
    string? Notes,
    Guid AddedById,
    Guid? LastModifiedById
);
