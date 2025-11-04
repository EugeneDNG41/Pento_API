using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;

namespace Pento.Application.RecipeIngredients.Create;
internal sealed class CreateRecipeIngredientCommandHandler(
    IRecipeIngredientRepository recipeIngredientRepository,
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitsRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateRecipeIngredientCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateRecipeIngredientCommand command,
        CancellationToken cancellationToken)
    {
        Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
        }

        FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(command.FoodRefId, cancellationToken);
        if (foodRef is null)
        {
            return Result.Failure<Guid>(FoodReferenceErrors.NotFound);
        }
        Unit? unit = await unitsRepository.GetByIdAsync(command.UnitId, cancellationToken);
        if(unit is null)
        {
            return Result.Failure<Guid>(UnitErrors.NotFound);
        }


        DateTime utcNow = DateTime.UtcNow;

        var ingredient = RecipeIngredient.Create(
            command.RecipeId,
            command.FoodRefId,
            command.Quantity,
            command.UnitId,
            command.Notes,
            utcNow
        );

        await recipeIngredientRepository.AddAsync(ingredient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ingredient.Id);
    }
}
