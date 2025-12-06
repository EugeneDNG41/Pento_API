using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.Recipes.Reserve;

internal sealed class FulfillRecipeReservationCommandHandler(
    IGenericRepository<FoodItemRecipeReservation> reservationRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IConverterService converter,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<FulfillRecipeReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(FulfillRecipeReservationCommand command, CancellationToken cancellationToken)
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

        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(reservation.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        decimal reservedQty = reservation.Quantity; 
        decimal newQty = command.NewQuantity;

        decimal newQtyInItemUnit = newQty;

        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> convertResult = await converter.ConvertAsync(
                newQty,
                fromUnitId: command.UnitId,
                toUnitId: foodItem.UnitId,
                cancellationToken);

            if (convertResult.IsFailure)
            {
                return Result.Failure<Guid>(convertResult.Error);
            }

            newQtyInItemUnit = convertResult.Value;
        }

        if (newQtyInItemUnit < reservedQty)
        {
            decimal returnQty = reservedQty - newQtyInItemUnit;
            foodItem.CancelReservation(returnQty, reservation.Id);
        }
        else if (newQtyInItemUnit > reservedQty)
        {
            decimal additionalQty = newQtyInItemUnit - reservedQty;

            if (additionalQty > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }

            foodItem.Reserve(additionalQty, command.NewQuantity, command.UnitId, userContext.UserId);
        }

        reservation.MarkAsFulfilled(command.NewQuantity, command.UnitId, userContext.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
