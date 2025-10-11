using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.Recipes;

namespace Pento.Infrastructure.Repositories;
internal sealed class RecipeRepository : Repository<Recipe>, IRecipeRepository
{
    public RecipeRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }



    public async Task<List<Recipe>> GetAllPublicAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Recipe>()
            .Where(r => r.IsPublic)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Recipe>> GetByCreatorAsync(Guid createdBy, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Recipe>()
            .Where(r => r.CreatedBy == createdBy)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<Recipe>().AddAsync(recipe, cancellationToken);
    }

    public void Remove(Recipe recipe)
    {
        DbContext.Set<Recipe>().Remove(recipe);
    }
}
