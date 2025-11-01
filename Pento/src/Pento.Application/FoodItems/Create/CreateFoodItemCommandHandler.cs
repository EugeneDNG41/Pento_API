using JasperFx.Events.Daemon;
using Marten;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Users;

namespace Pento.Application.FoodItems.Create;

internal sealed class CreateFoodItemCommandHandler(IFoodItemRepository repository)
    : ICommandHandler<CreateFoodItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFoodItemCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure<Guid>(UserErrors.NotInAnyHouseHold);
        }
        var e = new FoodItemAdded(
            Guid.CreateVersion7(),
            command.FoodRefId,
            command.CompartmentId,
            command.HouseholdId.Value,
            command.CustomName,
            command.Quantity,
            command.UnitId,
            command.ExpirationDate.ToUniversalTime(),
            command.Notes);
        await repository.StartStreamAsync(e, cancellationToken);
        return e.Id;
    }
}
