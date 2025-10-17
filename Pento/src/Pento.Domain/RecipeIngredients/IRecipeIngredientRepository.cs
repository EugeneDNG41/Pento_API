using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.RecipeIngredients;
public interface IRecipeIngredientRepository
{
    Task<RecipeIngredient?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<RecipeIngredient>> GetByRecipeIdAsync(
        Guid recipeId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        RecipeIngredient recipeIngredient,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        RecipeIngredient recipeIngredient,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}

