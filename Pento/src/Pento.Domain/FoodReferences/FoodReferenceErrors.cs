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

    public static readonly Error Conflict = Error.Conflict(
        "FoodReferences.Conflict",
        "A food reference with the same name or code already exists."
    );

}
