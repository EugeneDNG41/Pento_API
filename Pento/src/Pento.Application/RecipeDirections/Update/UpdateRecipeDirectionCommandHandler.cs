using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;

namespace Pento.Application.RecipeDirections.Update;

internal sealed class UpdateRecipeDirectionCommandHandler(
    IGenericRepository<RecipeDirection> recipeDirectionRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateRecipeDirectionCommand>
{
    public async Task<Result> Handle(UpdateRecipeDirectionCommand command, CancellationToken cancellationToken)
    {
        RecipeDirection? direction = await recipeDirectionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (direction is null)
        {
            return Result.Failure(RecipeDirectionErrors.NotFound(command.Id));
        }

        direction.Update(command.Description,DateTime.UtcNow);

        recipeDirectionRepository.Update(direction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
