using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeDirections;

public static class RecipeDirectionErrors
{
    public static Error NotFound =>
        Error.NotFound("RecipeDirections.IdentityNotFound", $"The recipe direction was not found.");
    public static Error DupicateDirectionStep(int step) =>
        Error.Problem("RecipeDirections.DuplicateDirection", $"The recipe direction with step {step} already exists.");
}
