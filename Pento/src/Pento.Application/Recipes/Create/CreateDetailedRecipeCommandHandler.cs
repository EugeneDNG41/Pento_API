using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.RecipeDirections;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.Recipes.Create;
internal sealed class CreateDetailedRecipeCommandHandler(
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<RecipeIngredient> recipeIngredientRepository,
    IGenericRepository<RecipeDirection> recipeDirectionRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository,
    IUserContext userContext,
    IBlobService blobService,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateDetailedRecipeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDetailedRecipeCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        if (userId == Guid.Empty)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        Uri? recipeImageUrl = null;

        if (command.Image is not null)
        {
            Result<Uri> upload = await blobService.UploadImageFromUrlAsync(
                source: command.Image,
                "recipes",
                cancellationToken
            );

            if (upload.IsFailure)
            {
                return Result.Failure<Guid>(upload.Error);
            }

            recipeImageUrl = upload.Value;
        }

        var time = TimeRequirement.Create(command.PrepTimeMinutes, command.CookTimeMinutes);

        var recipe = Recipe.Create(
            command.Title,
            command.Description,
            time,
            command.Notes,
            command.Servings,
            command.DifficultyLevel,
            recipeImageUrl,   
            userId,
            command.IsPublic,
            DateTime.UtcNow
        );

        recipeRepository.Add(recipe);


        foreach (RecipeIngredientRequest item in command.Ingredients)
        {
            FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(item.FoodRefId, cancellationToken);
            if (foodRef is null)
            {
                return Result.Failure<Guid>(FoodReferenceErrors.NotFound);
            }

            Unit? unit = await unitRepository.GetByIdAsync(item.UnitId, cancellationToken);
            if (unit is null)
            {
                return Result.Failure<Guid>(UnitErrors.NotFound);
            }

            var ingredient = RecipeIngredient.Create(
                recipe.Id,
                item.FoodRefId,
                item.Quantity,
                item.UnitId,
                item.Notes,
                DateTime.UtcNow
            );

            recipeIngredientRepository.Add(ingredient);
        }


        foreach (RecipeDirectionRequest? dir in command.Directions.OrderBy(d => d.StepNumber))
        {
            Uri? directionImageUrl = null;

            if (dir.Image is not null)
            {
                Result<Uri> uploaded = await blobService.UploadImageFromUrlAsync(
                    source: dir.Image,
                    "recipesSteps",
                    cancellationToken
                );

                if (uploaded.IsFailure)
                {
                    return Result.Failure<Guid>(uploaded.Error);
                }

                directionImageUrl = uploaded.Value;
            }

            var direction = RecipeDirection.Create(
                recipe.Id,
                dir.StepNumber,
                dir.Description,
                directionImageUrl,    
                DateTime.UtcNow
            );

            recipeDirectionRepository.Add(direction);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(recipe.Id);
    }
}
