using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.MealPlans;

namespace Pento.Infrastructure.Repositories;
internal sealed class MealPlanRepository : Repository<MealPlan>, IMealPlanRepository
{
    public MealPlanRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<MealPlan?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<MealPlan>()
            .FirstOrDefaultAsync(
                m =>  m.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(
        MealPlan mealPlan,
        CancellationToken cancellationToken = default)
    {
        await DbContext.Set<MealPlan>().AddAsync(mealPlan, cancellationToken);
    }

    public async Task UpdateAsync(
        MealPlan mealPlan,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<MealPlan>().Update(mealPlan);
        await Task.CompletedTask;
    }
}
