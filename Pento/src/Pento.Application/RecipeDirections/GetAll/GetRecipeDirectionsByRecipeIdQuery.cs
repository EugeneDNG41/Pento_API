using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.GetAll;

public sealed record GetRecipeDirectionsByRecipeIdQuery(Guid RecipeId)
    : IQuery<RecipeWithDirectionsResponse>;
