using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.UpdateExpirationDate;

internal sealed class UpdateFoodItemExpirationDateCommandHandler(
    IUserContext userContext,
    IDocumentSession session) : ICommandHandler<UpdateFoodItemExpirationDateCommand>
{
    public async Task<Result> Handle(UpdateFoodItemExpirationDateCommand command, CancellationToken cancellationToken)
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
        DateTime newExpirationDateUtc = command.NewExpirationDate.ToUniversalTime();
        if (foodItem.ExpirationDateUtc == newExpirationDateUtc)
        {
            return Result.Success();
        }
        await session.Events.AppendOptimistic(command.Id, new FoodItemExpirationDateUpdated(newExpirationDateUtc));
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
