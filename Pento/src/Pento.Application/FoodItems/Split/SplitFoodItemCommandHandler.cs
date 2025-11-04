using JasperFx.Events;
using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Split;

internal sealed class SplitFoodItemCommandHandler(
    IUserContext userContext,
    IDocumentSession session) : ICommandHandler<SplitFoodItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(SplitFoodItemCommand command, CancellationToken cancellationToken)
    {
        IEventStream<FoodItem> stream = await session.Events.FetchForWriting<FoodItem>(command.Id, command.Version, cancellationToken);
        FoodItemDetail? projection = await session.Events.FetchLatest<FoodItemDetail>(command.Id, cancellationToken);
        FoodItem? foodItem = stream.Aggregate;
        if (foodItem is null || projection is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
        }
        if (command.Quantity >= foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }
        await session.Events.AppendOptimistic(command.Id, new FoodItemSplit(command.Quantity));

        var added = new FoodItemAdded(
            Guid.CreateVersion7(),
            foodItem.FoodReferenceId,
            foodItem.CompartmentId,
            projection.CompartmentName,
            foodItem.HouseholdId,
            foodItem.Name,
            foodItem.ImageUrl,
            command.Quantity,
            projection.UnitAbbreviation,
            foodItem.UnitId,
            foodItem.ExpirationDateUtc,
            foodItem.Notes,
            foodItem.Id);
        session.Events.StartStream<FoodItem>(added.Id, added);
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return added.Id;
    }
}
