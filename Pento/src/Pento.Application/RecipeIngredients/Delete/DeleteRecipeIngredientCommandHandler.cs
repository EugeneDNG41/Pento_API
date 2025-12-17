using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients;

namespace Pento.Application.RecipeIngredients.Delete;

internal sealed class DeleteRecipeIngredientCommandHandler(
    IGenericRepository<RecipeIngredient> recipeIngredientRepository,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<DeleteRecipeIngredientCommand>
{
    public async Task<Result> Handle(DeleteRecipeIngredientCommand command, CancellationToken cancellationToken)
    {
        RecipeIngredient? recipeIngredient = await recipeIngredientRepository.GetByIdAsync(command.RecipeIngredientId, cancellationToken);
        if (recipeIngredient is null)
        {
            return Result.Failure(RecipeIngredientErrors.NotFound);
        }

        await recipeIngredientRepository.RemoveAsync(recipeIngredient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
