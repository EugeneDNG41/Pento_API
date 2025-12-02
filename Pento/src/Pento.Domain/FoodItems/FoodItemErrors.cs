using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;

namespace Pento.Domain.FoodItems;

public static class FoodItemErrors
{
    public static readonly Error NotFound =
        Error.NotFound("FoodItems.NotFound", "Food item not found");
    public static readonly Error ForbiddenAccess =
        Error.Forbidden("FoodItems.ForbiddenAccess", "You do not have access to this food item");
    public static readonly Error InsufficientQuantity =
        Error.Problem("FoodItems.InsufficientQuantity", "Insufficient quantity available");
    public static readonly Error NotSameType =
        Error.Problem("FoodItems.NotSameType", "Food items are not of the same type for merging");
    public static readonly Error InvalidMeasurementUnit =
        Error.Problem("FoodItems.InvalidMeasurementUnit", "The specified measurement unit is not valid for this food item");
    public static readonly Error HasPendingReservation =
        Error.Conflict("FoodItems.HasPendingReservation", "The food item has pending reservations and cannot be deleted"); //business rule
}
