using Pento.Application.Abstractions.Messaging;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.AdjustQuantity;

public sealed record DiscardFoodItemCommand(Guid Id, decimal Quantity, DiscardReason Reason, int Version) : ICommand;
