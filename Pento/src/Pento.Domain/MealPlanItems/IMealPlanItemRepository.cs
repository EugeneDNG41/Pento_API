using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.MealPlanItems;
public interface IMealPlanItemRepository
{
    Task<MealPlanItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MealPlanItem>> GetByMealPlanIdAsync(Guid mealPlanId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MealPlanItem>> GetByRecipeIdAsync(Guid recipeId, CancellationToken cancellationToken = default);
    Task AddAsync(MealPlanItem mealPlanItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(MealPlanItem mealPlanItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}
