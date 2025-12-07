using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Update;

public sealed record UpdateRecipeDirectionImageCommand(
    Guid RecipeDirectionId,
    IFormFile ImageFile
) : ICommand<string>;
