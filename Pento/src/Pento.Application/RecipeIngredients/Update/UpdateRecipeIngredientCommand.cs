using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeIngredients.Update;

public sealed record UpdateRecipeIngredientCommand(Guid Id, decimal Quantity, Guid UnitId, string? Notes) : ICommand;

