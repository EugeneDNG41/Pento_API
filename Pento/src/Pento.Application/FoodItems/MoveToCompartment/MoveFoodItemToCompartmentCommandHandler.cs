using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Application.FoodItems.MoveToCompartment;

internal sealed class MoveFoodItemToCompartmentCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IDocumentSession session) : ICommandHandler<MoveFoodItemToCompartmentCommand>
{
    public async Task<Result> Handle(MoveFoodItemToCompartmentCommand command, CancellationToken cancellationToken)
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
        if (foodItem.CompartmentId == command.NewCompartmentId)
        {
            return Result.Success();
        }
        Compartment? newCompartment = await compartmentRepository.GetByIdAsync(command.NewCompartmentId, cancellationToken);
        if (newCompartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound);
        }
        if (newCompartment.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(CompartmentErrors.ForbiddenAccess);
        }
        await session.Events.AppendOptimistic(command.Id, new FoodItemCompartmentMoved(command.NewCompartmentId, newCompartment.Name)); //make sure to check if different storage type later
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
