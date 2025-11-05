using Marten;
using Marten.Events;
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
    IDocumentSession session) : ICommandHandler<UploadFoodItemImageCommand, Uri>
{
    public async Task<Result<Uri>> Handle(UploadFoodItemImageCommand command, CancellationToken cancellationToken)
    {
        IEventStream<FoodItem> stream = await session.Events.FetchForWriting<FoodItem>(command.Id, command.Version, cancellationToken);
        FoodItem? foodItem = stream.Aggregate;
        if (foodItem is null)
        {
            return Result.Failure<Uri>(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<Uri>(FoodItemErrors.ForbiddenAccess);
        }
        if (command.File is not null)
        {
            Result<Uri> uploadResult = await blobService.UploadImageAsync(command.File, nameof(FoodItem), cancellationToken);
            if (uploadResult.IsFailure)
            {
                return Result.Failure<Uri>(uploadResult.Error);
            }
            await session.Events.AppendOptimistic(command.Id, new FoodItemImageUpdated(uploadResult.Value));
            session.LastModifiedBy = userContext.UserId.ToString();
            await session.SaveChangesAsync(cancellationToken);
            return uploadResult.Value;
        } 
        else
        {
            FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
            if (foodReference is null)
            {
                return Result.Failure<Uri>(FoodReferenceErrors.NotFound);
            }
            if (foodReference.ImageUrl is not null && foodItem.ImageUrl is not null && foodItem.ImageUrl != foodReference.ImageUrl)
            {
                await blobService.DeleteImageAsync(nameof(FoodItem), foodItem.ImageUrl.AbsoluteUri, cancellationToken);
            }
            await session.Events.AppendOptimistic(command.Id, new FoodItemImageUpdated(foodReference.ImageUrl));
            session.LastModifiedBy = userContext.UserId.ToString();
            await session.SaveChangesAsync(cancellationToken);
            return foodReference.ImageUrl;
        }   
    }
}
