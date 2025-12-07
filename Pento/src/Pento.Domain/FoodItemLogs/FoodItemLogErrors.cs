using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodItemLogs;

public static class FoodItemLogErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "FoodItemLog.NotFound",
        description: "Food item log not found.");
}
