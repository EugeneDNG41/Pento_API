using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.AdjustQuantity;

public sealed record AdjustFoodItemQuantityCommand(Guid Id, decimal Quantity, int Version) : ICommand;
