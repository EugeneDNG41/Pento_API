using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.MealPlanItems;

namespace Pento.Infrastructure.Repositories;
internal sealed class MealPlanItemRepository : Repository<MealPlanItem>, IMealPlanItemRepository
{
    public MealPlanItemRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }


    public async Task<IReadOnlyList<MealPlanItem>> GetByMealPlanIdAsync(
        Guid mealPlanId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<MealPlanItem>()
            .Where(item => item.MealPlanId == mealPlanId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MealPlanItem>> GetByRecipeIdAsync(
        Guid recipeId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<MealPlanItem>()
            .Where(item => item.RecipeId == recipeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        MealPlanItem mealPlanItem,
        CancellationToken cancellationToken = default)
    {
        await DbContext.Set<MealPlanItem>().AddAsync(mealPlanItem, cancellationToken);
    }

    public async Task UpdateAsync(
        MealPlanItem mealPlanItem,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<MealPlanItem>().Update(mealPlanItem);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        MealPlanItem? entity = await DbContext.Set<MealPlanItem>()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (entity is not null)
        {
            DbContext.Set<MealPlanItem>().Remove(entity);
        }

        await Task.CompletedTask;
    }
}
