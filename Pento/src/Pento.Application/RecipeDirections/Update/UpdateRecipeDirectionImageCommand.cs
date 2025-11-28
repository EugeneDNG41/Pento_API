using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Update;
public sealed record UpdateRecipeDirectionImageCommand(
    Guid RecipeDirectionId,
    IFormFile ImageFile
) : ICommand<string>;
