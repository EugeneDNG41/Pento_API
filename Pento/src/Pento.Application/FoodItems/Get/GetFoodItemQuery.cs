
using Pento.Application.Abstractions.Messaging;
namespace Pento.Application.FoodItems.Get;

public sealed record GetFoodItemQuery(Guid Id) : IQuery<FoodItemResponse>;
