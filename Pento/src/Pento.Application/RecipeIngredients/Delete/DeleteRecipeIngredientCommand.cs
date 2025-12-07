using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeIngredients.Delete;

public sealed record DeleteRecipeIngredientCommand(Guid RecipeIngredientId) : ICommand;

