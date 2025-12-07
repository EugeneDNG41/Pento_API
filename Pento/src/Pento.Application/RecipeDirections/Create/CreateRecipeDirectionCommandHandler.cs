using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;
using Pento.Domain.Recipes;

namespace Pento.Application.RecipeDirections.Create;

internal sealed class CreateRecipeDirectionCommandHandler(
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<RecipeDirection> recipeDirectionRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateRecipeDirectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRecipeDirectionCommand command, CancellationToken cancellationToken)
    {
        Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
        }
        IEnumerable<RecipeDirection>? directions = await recipeDirectionRepository.FindAsync(
            d => d.RecipeId == command.RecipeId && d.StepNumber == command.StepNumber,
            cancellationToken
        );
        if (directions is not null)
        {
            return Result.Failure<Guid>(RecipeDirectionErrors.DupicateDirectionStep(command.StepNumber));
        }

        var direction = RecipeDirection.Create(
               command.RecipeId,
               command.StepNumber,
               command.Description,
               command.ImageUrl,
               DateTime.UtcNow);


        recipeDirectionRepository.Add(direction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(direction.Id);
    }
}
