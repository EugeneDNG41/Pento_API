using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans.Events;

namespace Pento.Domain.MealPlans;
public sealed class MealPlan : Entity
{
    private MealPlan() { }

    public MealPlan(
        Guid id,
        Guid householdId,
        string name,
        MealType mealType,
        DateOnly scheduledDate,
        int servings,
        string? notes,
        Guid createdBy,
        DateTime createdOnUtc)
        : base(id)
    {
        HouseholdId = householdId;
        Name = name;
        MealType = mealType;
        ScheduledDate = scheduledDate;
        Servings = servings > 0 ? servings : 1;
        Notes = notes;
        CreatedBy = createdBy;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    public Guid HouseholdId { get; private set; }
    public string Name { get; private set; } = null!;
    public MealType MealType { get; private set; }
    public DateOnly ScheduledDate { get; private set; }
    public int Servings { get; private set; }
    public string? Notes { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }

    public static MealPlan Create(
        Guid householdId,
        string name,
        MealType mealType,
        DateOnly scheduledDate,
        int servings,
        string? notes,
        Guid createdBy,
        DateTime utcNow)
    {
        var meal = new MealPlan(
            Guid.CreateVersion7(),
            householdId,
            name,
            mealType,
            scheduledDate,
            servings,
            notes,
            createdBy,
            utcNow);

        meal.Raise(new MealPlanCreatedDomainEvent(meal.Id, createdBy));
        return meal;
    }

    public void Update(
        MealType mealType,
        DateOnly scheduledDate,
        int servings,
        string? notes,
        DateTime utcNow)
    {
        MealType = mealType;
        ScheduledDate = scheduledDate;
        Servings = servings > 0 ? servings : 1;
        Notes = notes;
        UpdatedOnUtc = utcNow;

        Raise(new MealPlanUpdatedDomainEvent(Id));
    }
}
