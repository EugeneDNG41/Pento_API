using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.PossibleUnits;

public static class PossibleUnitErrors
{
    public static Error NotFound(Guid possibleUnitId) =>
        Error.NotFound(
            "PossibleUnits.NotFound",
            $"The possible unit with ID {possibleUnitId} was not found."
        );

    public static Error InvalidFoodReference(Guid foodRefId) =>
        Error.Problem(
            "PossibleUnits.InvalidFoodReference",
            $"The provided FoodReference ID {foodRefId} is invalid or does not exist."
        );

    public static Error InvalidUnit(Guid unitId) =>
        Error.Problem(
            "PossibleUnits.InvalidUnit",
            $"The provided Unit ID {unitId} is invalid or does not exist."
        );

    public static readonly Error Conflict = Error.Conflict(
        "PossibleUnits.Conflict",
        "A possible unit with the same FoodReference and Unit already exists."
    );
    public static readonly Error NoDefaultUnit = Error.Failure(
        "PossibleUnits.NoDefaultUnit",
        "No default possible unit is defined for the specified food reference."
    );
}
