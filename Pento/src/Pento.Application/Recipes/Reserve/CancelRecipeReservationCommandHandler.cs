using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.Households;

namespace Pento.Application.Recipes.Reserve;

internal sealed class CancelRecipeReservationCommandHandler(
    IGenericRepository<FoodItemRecipeReservation> reservationRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CancelRecipeReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CancelRecipeReservationCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        FoodItemRecipeReservation? reservation = await reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.NotFound);
        }

        if (reservation.HouseholdId != householdId)
        {
            return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.InvalidState);
        }

        var foodItem = await foodItemRepository.GetByIdAsync(reservation.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        foodItem.CancelReservation(reservation.Quantity, reservation.Id);

        reservation.MarkAsCancelled();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
