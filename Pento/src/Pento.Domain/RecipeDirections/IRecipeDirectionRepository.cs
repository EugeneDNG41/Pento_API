using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.RecipeDirections;
public interface IRecipeDirectionRepository
{
    Task<RecipeDirection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RecipeDirection>> GetByRecipeIdAsync(Guid recipeId, CancellationToken cancellationToken = default);
    Task AddAsync(RecipeDirection direction, CancellationToken cancellationToken = default);
    Task UpdateAsync(RecipeDirection direction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RecipeDirection?> GetByStep(int step, CancellationToken cancellationToken = default);
}
