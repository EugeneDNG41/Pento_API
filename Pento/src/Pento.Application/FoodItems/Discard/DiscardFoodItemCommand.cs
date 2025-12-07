using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Discard;

public sealed record DiscardFoodItemCommand(Guid Id, decimal Quantity, Guid UnitId) : ICommand;
