using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Recipes;
public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Recipe>> GetAllPublicAsync(CancellationToken cancellationToken = default);

    Task<List<Recipe>> GetByCreatorAsync(Guid createdBy, CancellationToken cancellationToken = default);

    Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default);

    void Remove(Recipe recipe);
}
