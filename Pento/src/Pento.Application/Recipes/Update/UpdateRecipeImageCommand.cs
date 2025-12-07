using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Update;

public sealed record UpdateRecipeImageCommand(
    Guid RecipeId,
    IFormFile ImageFile
) : ICommand<string>;
