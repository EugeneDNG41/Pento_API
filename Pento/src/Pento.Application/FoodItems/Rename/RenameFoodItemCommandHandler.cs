using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.Rename;

internal sealed class RenameFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IDocumentSession session) : ICommandHandler<RenameFoodItemCommand>
{
    public async Task<Result> Handle(RenameFoodItemCommand command, CancellationToken cancellationToken)
    {
        IEventStream<FoodItem> stream = await session.Events.FetchForWriting<FoodItem>(command.Id, command.Version, cancellationToken);
        FoodItem? foodItem = stream.Aggregate;
        if (foodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        if (foodItem.Name == command.Name)
        {
            return Result.Success();
        }
        if (string.IsNullOrEmpty(command.Name))
        {
            FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
            if (foodReference is null)
            {
                return Result.Failure(FoodReferenceErrors.NotFound);
            }
            await session.Events.AppendOptimistic(command.Id, new FoodItemRenamed(foodReference.Name));
        } 
        else
        {
            await session.Events.AppendOptimistic(command.Id, new FoodItemRenamed(command.Name));
        }
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
