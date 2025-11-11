using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.File;
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
        Uri? oldImageUrl = foodItem.ImageUrl;
        if (command.File is not null)
        {
            Result<Uri> uploadResult = await blobService.UploadImageAsync(command.File, nameof(FoodItem), cancellationToken);
            if (uploadResult.IsFailure)
            {
                return Result.Failure<Uri>(uploadResult.Error);
            }
            foodItem.UpdateImageUrl(uploadResult.Value, userContext.UserId);            
        } 
        else
        {
            foodItem.UpdateImageUrl(foodReference.ImageUrl, userContext.UserId);
        }
        if (foodReference.ImageUrl is not null && oldImageUrl is not null && oldImageUrl != foodReference.ImageUrl)
        {
            await blobService.DeleteImageAsync(nameof(FoodItem), oldImageUrl.AbsoluteUri, cancellationToken);
        }
        foodItemRepository.Update(foodItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return foodReference.ImageUrl;
    }
}
