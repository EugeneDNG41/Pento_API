using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Create;
internal sealed class CreateRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateRecipeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateRecipeCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Failure<Guid>(RecipeErrors.InvalidTitle);
        }

        if (request.PrepTimeMinutes < 0 || request.CookTimeMinutes < 0)
        {
            return Result.Failure<Guid>(RecipeErrors.InvalidTime);
        }

        if (request.Servings is not null && request.Servings <= 0)
        {
            return Result.Failure<Guid>(RecipeErrors.InvalidServings);
        }


        var time = TimeRequirement.Create(request.PrepTimeMinutes, request.CookTimeMinutes);

        var recipe = Recipe.Create(
            request.Title,
            request.Description,
            time,
            request.Notes,
            request.Servings,
            request.DifficultyLevel,
            request.ImageUrl,
            request.CreatedBy,
            request.IsPublic,
            DateTime.UtcNow
        );

        await recipeRepository.AddAsync(recipe, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(recipe.Id);
    }
}
