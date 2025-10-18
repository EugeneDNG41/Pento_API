using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.RecipeDirections;

namespace Pento.Infrastructure.Repositories;
internal sealed class RecipeDirectionRepository
    : Repository<RecipeDirection>, IRecipeDirectionRepository
{
    public RecipeDirectionRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<RecipeDirection>> GetByRecipeIdAsync(
        Guid recipeId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<RecipeDirection>()
            .Where(x => x.RecipeId == recipeId)
            .OrderBy(x => x.StepNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        RecipeDirection direction,
        CancellationToken cancellationToken = default)
    {
        await DbContext.Set<RecipeDirection>().AddAsync(direction, cancellationToken);
    }

    public async Task UpdateAsync(
        RecipeDirection direction,
        CancellationToken cancellationToken = default)
    {
        DbContext.Set<RecipeDirection>().Update(direction);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RecipeDirection? entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            DbContext.Set<RecipeDirection>().Remove(entity);
        }
    }
}
