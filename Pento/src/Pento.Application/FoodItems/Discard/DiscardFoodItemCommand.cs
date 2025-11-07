using Pento.Application.Abstractions.Messaging;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.Discard;

public sealed record DiscardFoodItemCommand(Guid Id, decimal Quantity, DiscardReason Reason, int Version) : ICommand;
