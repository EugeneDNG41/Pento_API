using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Delete;

public sealed record DeleteFoodItemCommand(Guid FoodItemId) : ICommand;
