using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Merge;

public sealed record MergeFoodItemCommand(Guid SourceId, Guid TargetId, decimal Quantity, int SourceVersion, int TargetVersion) : ICommand;
