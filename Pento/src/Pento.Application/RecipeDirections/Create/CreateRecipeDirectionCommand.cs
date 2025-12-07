using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Create;

public sealed record CreateRecipeDirectionCommand(
    Guid RecipeId,
    int StepNumber,
    string Description,
    Uri? ImageUrl
) : ICommand<Guid>;
