using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeIngredients.GetAll;

public sealed record GetRecipeIngredientsByRecipeIdQuery(Guid RecipeId)
    : IQuery<RecipeWithIngredientsResponse>;
