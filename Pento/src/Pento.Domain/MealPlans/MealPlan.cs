using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlans;
public sealed class MealPlan : Entity
{
    public MealPlan(
        Guid id,
        Guid householdId,
        string name,
        Guid createdBy,
        DateRange duration,
        DateTime createdOnUtc)
        : base(id)
    {
        HouseholdId = householdId;
        Name = name;
        CreatedBy = createdBy;
        Duration = duration;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private MealPlan()
    {
    }

    public Guid HouseholdId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateRange Duration { get; private set; }
    public Guid CreatedBy { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }
}


