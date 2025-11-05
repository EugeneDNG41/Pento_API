using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Units;

namespace Pento.Application.RecipeIngredients.Update;
internal sealed class UpdateRecipeIngredientCommandHandler(
    IGenericRepository<RecipeIngredient> recipeIngredientRepository,
    IGenericRepository<Unit> UnitRepository,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<UpdateRecipeIngredientCommand>
{
    public async Task<Result> Handle(UpdateRecipeIngredientCommand command, CancellationToken cancellationToken)
    {
        RecipeIngredient? recipeIngredient = await recipeIngredientRepository.GetByIdAsync(command.Id, cancellationToken);
        if (recipeIngredient is null)
        {
            return Result.Failure(RecipeIngredientErrors.NotFound);
        }
        Unit? unit = await UnitRepository.GetByIdAsync(command.UnitId, cancellationToken);
        if (unit is null)
        {
            return Result.Failure(UnitErrors.NotFound);
        }
        recipeIngredient.UpdateDetails(
            command.Quantity,
            command.UnitId,
            command.Notes,
            DateTime.UtcNow
        );
        recipeIngredientRepository.Update(recipeIngredient);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
