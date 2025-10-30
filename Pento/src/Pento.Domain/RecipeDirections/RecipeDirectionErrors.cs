using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeDirections;

public static class RecipeDirectionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("RecipeDirections.IdentityNotFound", $"The recipe direction with ID {id} was not found.");

    public static readonly Error InvalidStepNumber = Error.Problem(
        "RecipeDirections.InvalidStepNumber",
        "Step number must be greater than 0."
    );

    public static readonly Error InvalidDescription = Error.Problem(
        "RecipeDirections.InvalidDescription",
        "Description cannot be empty or whitespace."
    );
}
