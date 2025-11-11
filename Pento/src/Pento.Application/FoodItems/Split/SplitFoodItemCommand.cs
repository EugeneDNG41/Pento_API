using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Split;

public sealed record SplitFoodItemCommand(Guid Id, decimal Quantity, Guid UnitId) : ICommand<Guid>;
