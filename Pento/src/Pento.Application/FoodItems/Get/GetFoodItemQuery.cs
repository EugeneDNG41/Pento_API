
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.FoodItems.Projections;
namespace Pento.Application.FoodItems.Get;

public sealed record GetFoodItemQuery(Guid Id) : IQuery<FoodItemDetail>;
