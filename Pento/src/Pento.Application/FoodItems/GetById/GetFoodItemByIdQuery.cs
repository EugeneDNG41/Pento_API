
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
namespace Pento.Application.FoodItems.GetById;

public sealed record GetFoodItemByIdQuery(Guid Id) : IQuery<FoodItemDetail>;
