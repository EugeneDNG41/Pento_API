using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;

namespace Pento.Application.RecipeDirections.Create;
internal sealed class CreateRecipeDirectionCommandHandler(
    IRecipeDirectionRepository recipeDirectionRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateRecipeDirectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRecipeDirectionCommand request, CancellationToken cancellationToken)
    {
        if (request.StepNumber <= 0)
        {
            return Result.Failure<Guid>(RecipeDirectionErrors.InvalidStepNumber);
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return Result.Failure<Guid>(RecipeDirectionErrors.InvalidDescription);
        }


        var direction = RecipeDirection.Create(
      request.RecipeId,
      request.StepNumber,
      request.Description,
      request.ImageUrl,
      DateTime.UtcNow
  );

        await recipeDirectionRepository.AddAsync(direction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(direction.Id);
    }
}
