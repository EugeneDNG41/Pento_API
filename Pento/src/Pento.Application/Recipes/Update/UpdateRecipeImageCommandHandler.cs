using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Update;

internal sealed class UpdateRecipeImageCommandHandler(
    IGenericRepository<Recipe> recipeRepository,
    IBlobService blobService,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateRecipeImageCommand, string>
{
    public async Task<Result<string>> Handle(
        UpdateRecipeImageCommand command,
        CancellationToken cancellationToken)
    {
        Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<string>(RecipeErrors.NotFound);
        }

        if (recipe.ImageUrl is not null)
        {
            string fileName = Path.GetFileName(recipe.ImageUrl.LocalPath);
            await blobService.DeleteImageAsync("recipes", fileName, cancellationToken);
        }

        Result<Uri> upload = await blobService.UploadImageAsync(
            command.ImageFile,
            "recipes",
            cancellationToken
        );

        if (upload.IsFailure)
        {
            return Result.Failure<string>(upload.Error);
        }

        recipe.UpdateImageUrl(upload.Value, DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(upload.Value.AbsoluteUri);
    }
}
