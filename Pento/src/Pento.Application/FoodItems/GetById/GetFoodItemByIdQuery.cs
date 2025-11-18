
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.GetById;

public sealed record GetFoodItemByIdQuery(Guid Id) : IQuery<FoodItemDetail>;
public sealed record BasicUserResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
);


