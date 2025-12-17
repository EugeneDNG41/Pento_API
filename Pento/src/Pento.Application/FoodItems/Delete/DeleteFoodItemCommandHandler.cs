using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;

namespace Pento.Application.FoodItems.Delete;

internal sealed class DeleteFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<FoodItemReservation> reservationRepository,
    IUnitOfWork unitOfWork,
    IHubContext<MessageHub, IMessageClient> hubContext
    ) : ICommandHandler<DeleteFoodItemCommand>
{
    public async Task<Result> Handle(DeleteFoodItemCommand command, CancellationToken cancellationToken)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.FoodItemId, cancellationToken);
        if (foodItem == null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        bool hasPendingReservations = await reservationRepository
            .AnyAsync(r => r.FoodItemId == foodItem.Id && r.Status == ReservationStatus.Pending, cancellationToken);
        if (hasPendingReservations)
        {
            return Result.Failure(FoodItemErrors.HasPendingReservation);
        }
        await foodItemRepository.RemoveAsync(foodItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(userContext.HouseholdId.Value.ToString())
            .FoodItemDeleted(command.FoodItemId);
        return Result.Success();
    }
}
