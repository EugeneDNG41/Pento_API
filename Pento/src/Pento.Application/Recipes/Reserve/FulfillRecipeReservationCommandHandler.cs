using Microsoft.AspNetCore.Mvc;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
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

        Result<decimal> newQtyInItemUnit = await converter.ConvertAsync(
                command.NewQuantity,
                fromUnitId: command.UnitId,
                toUnitId: foodItem.UnitId,
                cancellationToken);

        if (newQtyInItemUnit.IsFailure)
        {
            return Result.Failure<Guid>(newQtyInItemUnit.Error);
        }
        Result<decimal> reservedQtyInItemUnit = await converter.ConvertAsync(
                reservation.Quantity,
                fromUnitId: reservation.UnitId,
                toUnitId: foodItem.UnitId,
                cancellationToken);
        if (reservedQtyInItemUnit.IsFailure)
        {
            return Result.Failure<Guid>(reservedQtyInItemUnit.Error);
        }
        if (newQtyInItemUnit.Value < reservedQtyInItemUnit.Value)
        {
            decimal returnQty = reservedQtyInItemUnit.Value - newQtyInItemUnit.Value;
            foodItem.CancelReservation(returnQty, reservation.Id);
        }
        else if (newQtyInItemUnit.Value > reservedQtyInItemUnit.Value)
        {
            decimal additionalQty = newQtyInItemUnit.Value - reservedQtyInItemUnit.Value;

            if (additionalQty > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }

            foodItem.Reserve(additionalQty, additionalQty, foodItem.UnitId, userContext.UserId);
        }

        reservation.MarkAsFulfilled(command.NewQuantity, command.UnitId, userContext.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
