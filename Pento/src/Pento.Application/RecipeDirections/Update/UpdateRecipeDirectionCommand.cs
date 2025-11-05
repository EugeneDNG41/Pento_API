using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Update;

public sealed record UpdateRecipeDirectionCommand(
    Guid Id,
    string Description
    ) : ICommand;
