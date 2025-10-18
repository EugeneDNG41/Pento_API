using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.RecipeIngredients;

namespace Pento.Infrastructure.Repositories;
internal sealed class RecipeIngredientRepository
    : Repository<RecipeIngredient>, IRecipeIngredientRepository
{
    public RecipeIngredientRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<RecipeIngredient>()
            .Where(ri => ri.RecipeId == recipeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RecipeIngredient recipeIngredient, CancellationToken cancellationToken = default)
    {
        await DbContext.AddAsync(recipeIngredient, cancellationToken);
    }

    public async Task UpdateAsync(RecipeIngredient recipeIngredient, CancellationToken cancellationToken = default)
    {
        DbContext.Update(recipeIngredient);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RecipeIngredient entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            DbContext.Remove(entity);
        }
    }
}
