using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Consume;

public sealed record ConsumeFoodItemCommand(Guid Id, decimal Quantity, Guid UnitId) : ICommand;
