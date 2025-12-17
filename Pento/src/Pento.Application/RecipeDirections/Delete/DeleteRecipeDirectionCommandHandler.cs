using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;

namespace Pento.Application.RecipeDirections.Delete;

internal sealed class DeleteRecipeDirectionCommandHandler(
    IGenericRepository<RecipeDirection> recipeDirectionRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteRecipeDirectionCommand>
{
    public async Task<Result> Handle(DeleteRecipeDirectionCommand command, CancellationToken cancellationToken)
    {
        RecipeDirection? direction = await recipeDirectionRepository.GetByIdAsync(command.RecipeDirectionId, cancellationToken);

        if (direction is null)
        {
            return Result.Failure(RecipeDirectionErrors.NotFound);
        }
        await recipeDirectionRepository.RemoveAsync(direction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
