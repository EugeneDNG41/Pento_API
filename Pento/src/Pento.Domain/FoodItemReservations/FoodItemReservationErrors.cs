using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodItemReservations;

public static class FoodItemReservationErrors
{
    public static readonly Error NotFound =
        Error.NotFound("FoodItemReservations.NotFound", "Food item reservation not found");
    public static readonly Error InvalidState =
        Error.Problem("FoodItemReservations.InvalidState", "The food item reservation is not in a valid state for this operation");
    public static readonly Error MismatchWithIngredients =
        Error.Problem("FoodItemReservations.MismatchWithIngredients", "The number of food item reservations does not match the number of recipe ingredients");
}
