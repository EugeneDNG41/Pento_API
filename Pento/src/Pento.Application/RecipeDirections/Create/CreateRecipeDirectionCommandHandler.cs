using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JasperFx.Events.Daemon;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;
using Pento.Domain.Recipes;

namespace Pento.Application.RecipeDirections.Create;
internal sealed class CreateRecipeDirectionCommandHandler(
    IRecipeDirectionRepository recipeDirectionRepository,
    IGenericRepository<Recipe> recipeRepository, IUnitOfWork unitOfWork
) : ICommandHandler<CreateRecipeDirectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRecipeDirectionCommand command, CancellationToken cancellationToken)
    {
        Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
        }
        RecipeDirection? existingDirection = await recipeDirectionRepository.GetByStep(command.StepNumber, cancellationToken);
        if (existingDirection is not null)
        {
            return Result.Failure<Guid>(RecipeDirectionErrors.DupicateDirectionStep(command.StepNumber));
        }

        var direction = RecipeDirection.Create(
               command.RecipeId,
               command.StepNumber,
               command.Description,
               command.ImageUrl,
               DateTime.UtcNow);


        await recipeDirectionRepository.AddAsync(direction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(direction.Id);
    }
}
