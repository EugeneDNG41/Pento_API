using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.UploadImage;

internal sealed class UploadFoodItemImageCommandHandler(
    IUserContext userContext,
    IBlobService blobService,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UploadFoodItemImageCommand, Uri>
{
    public async Task<Result<Uri>> Handle(UploadFoodItemImageCommand command, CancellationToken cancellationToken)
    {

        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.Id, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Uri>(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<Uri>(FoodItemErrors.ForbiddenAccess);
        }
        FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
        if (foodReference is null)
        {
            return Result.Failure<Uri>(FoodReferenceErrors.NotFound);
        }
        if (command.File is not null)
        {
            Result<Uri> uploadResult = await blobService.UploadImageAsync(command.File, nameof(FoodItem), cancellationToken);
            if (uploadResult.IsFailure)
            {
                return Result.Failure<Uri>(uploadResult.Error);
            }
            if (foodItem.ImageUrl is not null && foodItem.ImageUrl != foodReference.ImageUrl)
            {
                Result deleteResult = await blobService.DeleteImageAsync(nameof(FoodItem), foodItem.ImageUrl.AbsoluteUri, cancellationToken);
                if (deleteResult.IsFailure)
                {
                    return Result.Failure<Uri>(deleteResult.Error);
                }
            }
            foodItem.UpdateImageUrl(uploadResult.Value, userContext.UserId);            
        } 
        else if (foodReference.ImageUrl is not null)
        {
            foodItem.UpdateImageUrl(foodReference.ImageUrl, userContext.UserId);
        }
        foodItemRepository.Update(foodItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return foodItem.ImageUrl;
    }
}
