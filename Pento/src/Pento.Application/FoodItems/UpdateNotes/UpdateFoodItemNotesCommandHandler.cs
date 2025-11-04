using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.UpdateNotes;

internal sealed class UpdateFoodItemNotesCommandHandler(
    IUserContext userContext,
    IDocumentSession session) : ICommandHandler<UpdateFoodItemNotesCommand>
{
    public async Task<Result> Handle(UpdateFoodItemNotesCommand command, CancellationToken cancellationToken)
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
        if (foodItem.Notes == command.NewNotes)
        {
            return Result.Success();
        }
        await session.Events.AppendOptimistic(command.Id, new FoodItemNotesUpdated(command.NewNotes));
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
