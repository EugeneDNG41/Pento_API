
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

namespace Pento.Application.Recipes.Create;
internal sealed class CreateDetailedRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    IRecipeIngredientRepository recipeIngredientRepository,
    IRecipeDirectionRepository recipeDirectionRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository,
    IUnitOfWork unitOfWork,
    IBlobService blobService, 
    IUserContext userContext    
) : ICommandHandler<CreateDetailedRecipeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDetailedRecipeCommand command, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        Uri? recipeImageUrl = null;
        if (command.ImageFile is not null)
        {
            Result<Uri> uploadResult = await blobService.UploadImageAsync(command.ImageFile, "recipes", cancellationToken);

            if (uploadResult.IsFailure)
            {
                return Result.Failure<Guid>(uploadResult.Error);
            }
            recipeImageUrl = uploadResult.Value;
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

        await recipeRepository.AddAsync(recipe, cancellationToken);

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
            await recipeIngredientRepository.AddAsync(ingredient, cancellationToken);
        }

        if (command.Directions != null)
        {
            foreach (RecipeDirectionRequest dir in command.Directions.OrderBy(d => d.StepNumber))
            {
                var direction = RecipeDirection.Create(
                    recipe.Id,
                    dir.StepNumber,
                    dir.Description,
                    dir.ImageUrl,
                    DateTime.UtcNow
                );
                await recipeDirectionRepository.AddAsync(direction, cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(recipe.Id);
    }
}
