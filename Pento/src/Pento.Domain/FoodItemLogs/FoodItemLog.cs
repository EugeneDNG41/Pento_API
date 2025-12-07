using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodItemLogs;

public sealed class FoodItemLog : Entity
{
    public FoodItemLog(
        Guid id,
        Guid foodItemId,
        Guid householdId,
        Guid userId,
        DateTime timestamp,
        FoodItemLogAction action,
        decimal quantity,
        Guid unitId)
    {
        Id = id;
        FoodItemId = foodItemId;
        HouseholdId = householdId;
        UserId = userId;
        Timestamp = timestamp;
        Action = action;
        Quantity = quantity;
        UnitId = unitId;
    }
    private FoodItemLog() { }
    public Guid FoodItemId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public FoodItemLogAction Action { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public static FoodItemLog Create(
        Guid foodItemId,
        Guid householdId,
        Guid userId,
        DateTime timestamp,
        FoodItemLogAction action,
        decimal quantity,
        Guid unitId)
    {
        return new FoodItemLog(
            Guid.CreateVersion7(),
            foodItemId,
            householdId,
            userId,
            timestamp,
            action,
            quantity,
            unitId);
    }
}
