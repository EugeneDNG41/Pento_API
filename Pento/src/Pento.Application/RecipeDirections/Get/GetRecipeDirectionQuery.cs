using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Get;

public sealed record GetRecipeDirectionQuery(Guid RecipeDirectionId)
    : IQuery<RecipeDirectionResponse>;
