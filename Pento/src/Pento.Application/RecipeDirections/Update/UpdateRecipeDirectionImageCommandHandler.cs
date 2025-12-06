using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;
using Pento.Domain.Recipes;

namespace Pento.Application.RecipeDirections.Update;

internal sealed class UpdateRecipeDirectionImageCommandHandler(
    IGenericRepository <RecipeDirection> directionRepository,
    IBlobService blobService,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateRecipeDirectionImageCommand, string>
{
    public async Task<Result<string>> Handle(UpdateRecipeDirectionImageCommand command, CancellationToken cancellationToken)
    {


        RecipeDirection? direction = await directionRepository.GetByIdAsync(command.RecipeDirectionId, cancellationToken);
        if (direction is null)
        {
            return Result.Failure<string>(RecipeErrors.NotFound);
        }

        if (direction.ImageUrl is not null)
        {
            await blobService.DeleteImageAsync("recipesSteps", Path.GetFileName(direction.ImageUrl.LocalPath), cancellationToken);
        }

        Result<Uri> upload = await blobService.UploadImageAsync(command.ImageFile, "recipesSteps", cancellationToken);
        if (upload.IsFailure)
        {
            return Result.Failure<string>(upload.Error);
        }

        Uri newUrl = upload.Value;

        direction.UpdateImageUrl(newUrl, DateTime.UtcNow);
        directionRepository.Update(direction);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(newUrl.AbsoluteUri);
    }
}
