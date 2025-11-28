using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Update;
public sealed record UpdateRecipeImageCommand(
    Guid RecipeId,
    IFormFile ImageFile
) : ICommand<string>;
