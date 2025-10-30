using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.MealPlans;
public interface IMealPlanRepository
{
    Task<MealPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MealPlan?> GetByNameAsync(Guid householdId, string name, CancellationToken cancellationToken = default);

    Task AddAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);

    Task UpdateAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);
}
