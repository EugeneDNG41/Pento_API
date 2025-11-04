using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Split;

public sealed record SplitFoodItemCommand(Guid Id, decimal Quantity, int Version) : ICommand<Guid>;
