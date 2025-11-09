using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodReferences;

public static class FoodReferenceErrors
{

    public static readonly Error NotFound =
        Error.NotFound("FoodReferences.NotFound", "Food reference not found");

    public static readonly Error InvalidGroup = Error.Problem(
        "FoodReferences.InvalidGroup",
        "The provided food group is invalid."
    );

    public static readonly Error InvalidName = Error.Problem(
        "FoodReferences.InvalidName",
        "The food reference name cannot be empty or whitespace."
    );

    public static readonly Error Conflict = Error.Conflict(
        "FoodReferences.Conflict",
        "A food reference with the same name or code already exists."
    );

}
