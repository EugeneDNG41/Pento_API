using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Create;

public sealed record CreateFoodItemBulkCommand(List<CreateFoodItemCommand> Commands) : ICommand;
