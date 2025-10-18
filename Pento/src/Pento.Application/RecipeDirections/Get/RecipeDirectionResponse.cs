using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.RecipeDirections.Get;
public sealed record RecipeDirectionResponse(
    Guid Id,
    Guid RecipeId,
    int StepNumber,
    string Description,
    Uri? ImageUrl,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
