using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

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
        UnitType unitType)
    {
        Id = id;
        FoodItemId = foodItemId;
        HouseholdId = householdId;
        UserId = userId;
        Timestamp = timestamp;
        Action = action;
        BaseQuantity = quantity;
        BaseUnitType = unitType;
    }
    private FoodItemLog() { }
    public Guid FoodItemId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Timestamp{ get; private set; }
    public FoodItemLogAction Action { get; private set; }
    public decimal BaseQuantity { get; private set; }
    public UnitType BaseUnitType { get; private set; }
    public static FoodItemLog Create(
        Guid foodItemId,
        Guid householdId,
        Guid userId,
        DateTime timestamp,
        FoodItemLogAction action,
        decimal quantity,
        UnitType unitType)
    {
        return new FoodItemLog(
            Guid.CreateVersion7(),
            foodItemId,
            householdId,
            userId,
            timestamp,
            action,
            quantity,
            unitType);
    }
}

public enum FoodItemLogAction
{
    Intake,
    Consumption,
    Reservation,
    Donation,
    Discard
}
