using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.Merge;
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable CS9113 // Sections of code should not be commented out
#pragma warning disable IDE0060 // Remove unused parameter
internal sealed class MergeFoodItemCommandHandler(
    IUserContext userContext) : ICommandHandler<MergeFoodItemCommand>
{
    public async Task<Result> Handle(MergeFoodItemCommand command, CancellationToken cancellationToken)
    {
        //IEventStream<FoodItem> sourceStream = await session.Events.FetchForWriting<FoodItem>(command.SourceId, command.SourceVersion, cancellationToken);
        //FoodItem? sourceFoodItem = sourceStream.Aggregate;
        //if (sourceFoodItem is null)
        //{
        //    return Result.Failure(FoodItemErrors.NotFound);
        //}
        //IEventStream<FoodItem> targetStream = await session.Events.FetchForWriting<FoodItem>(command.TargetId, command.TargetVersion, cancellationToken);
        //FoodItem? targetFoodItem = targetStream.Aggregate;
        //if (targetFoodItem is null)
        //{
        //    return Result.Failure(FoodItemErrors.NotFound);
        //}
        //if (sourceFoodItem.HouseholdId != userContext.HouseholdId ||
        //    targetFoodItem.HouseholdId != userContext.HouseholdId)
        //{
        //    return Result.Failure(FoodItemErrors.ForbiddenAccess);
        //}
        //if (sourceFoodItem.FoodReferenceId != targetFoodItem.FoodReferenceId)
        //{
        //    return Result.Failure(FoodItemErrors.NotSameType);
        //}
        //if (command.Quantity > sourceFoodItem.Quantity)
        //{
        //    return Result.Failure(FoodItemErrors.InsufficientQuantity);
        //}
        //await session.Events.AppendOptimistic(command.SourceId, new FoodItemRemovedByMerge(command.TargetId, command.Quantity));
        //await session.Events.AppendOptimistic(command.TargetId, new FoodItemMerged(command.SourceId, command.Quantity));
        //session.LastModifiedBy = userContext.UserId.ToString();
        //await session.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
