using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.Merge;
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable CS9113 // Sections of code should not be commented out
#pragma warning disable IDE0060 // Remove unused parameter
internal sealed class MergeFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<MergeFoodItemCommand>
{
    public async Task<Result> Handle(MergeFoodItemCommand command, CancellationToken cancellationToken)
    {
        FoodItem? sourceFoodItem = await foodItemRepository.GetByIdAsync(command.SourceId, cancellationToken);
        if (sourceFoodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        FoodItem? targetFoodItem = await foodItemRepository.GetByIdAsync(command.TargetId, cancellationToken);
        if (targetFoodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (sourceFoodItem.HouseholdId != userContext.HouseholdId ||
            targetFoodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        if (sourceFoodItem.FoodReferenceId != targetFoodItem.FoodReferenceId)
        {
            return Result.Failure(FoodItemErrors.NotSameType);
        }
        if (command.Quantity > sourceFoodItem.Quantity)
        {
            return Result.Failure(FoodItemErrors.InsufficientQuantity);
        }
        sourceFoodItem.AdjustQuantity(sourceFoodItem.Quantity - command.Quantity, userContext.UserId);
        targetFoodItem.AdjustQuantity(targetFoodItem.Quantity + command.Quantity, userContext.UserId);
        return Result.Success();
    }
}
