using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.Delete;

public sealed record DeleteFoodReferenceCommand(Guid FoodReferenceId) : ICommand;

