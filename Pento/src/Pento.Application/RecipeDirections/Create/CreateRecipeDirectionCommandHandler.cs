using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;
using Pento.Domain.Recipes;

namespace Pento.Application.RecipeDirections.Create;

internal sealed class CreateRecipeDirectionCommandHandler(
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<RecipeDirection> recipeDirectionRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateRecipeDirectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateRecipeDirectionCommand command,
        CancellationToken cancellationToken)
    {
        Recipe? recipe = await recipeRepository.GetByIdAsync(
            command.RecipeId,
            cancellationToken);

        if (recipe is null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
        }

        int nextStepNumber = 1;

        IEnumerable<RecipeDirection> existingDirections =
            await recipeDirectionRepository.FindAsync(
                d => d.RecipeId == command.RecipeId,
                cancellationToken);

        if (existingDirections.Any())
        {
            nextStepNumber = existingDirections.Max(d => d.StepNumber) + 1;
        }

        var direction = RecipeDirection.Create(
            recipeId: command.RecipeId,
            stepNumber: nextStepNumber,
            description: command.Description,
            imageUrl: command.ImageUrl,
            dateTimeProvider.UtcNow
        );

        recipeDirectionRepository.Add(direction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(direction.Id);
    }
}
