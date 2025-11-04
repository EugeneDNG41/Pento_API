using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Rename;

public sealed record RenameFoodItemCommand(Guid Id, string? Name, int Version) : ICommand;
