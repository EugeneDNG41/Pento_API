using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Delete;

public sealed record DeleteRecipeDirectionCommand(Guid RecipeDirectionId) : ICommand;
