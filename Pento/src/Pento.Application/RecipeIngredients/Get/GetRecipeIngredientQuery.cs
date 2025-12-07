using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeIngredients.Get;

public sealed record GetRecipeIngredientQuery(Guid RecipeIngredientId) : IQuery<RecipeIngredientResponse>
{
}
