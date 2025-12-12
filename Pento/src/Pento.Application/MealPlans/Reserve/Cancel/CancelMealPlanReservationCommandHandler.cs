using System;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve.Cancel;

internal sealed class CancelMealPlanReservationCommandHandler(
    IConverterService converter,
    IGenericRepository<FoodItemMealPlanReservation> reservationRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CancelMealPlanReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CancelMealPlanReservationCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        FoodItemMealPlanReservation? reservation =
            await reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);

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

        FoodItem? foodItem =
            await foodItemRepository.GetByIdAsync(reservation.FoodItemId, cancellationToken);

        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }
        Result<decimal> qtyInItemUnit = await converter.ConvertAsync(
                reservation.Quantity,
                reservation.UnitId,
                foodItem.UnitId,
                cancellationToken
            );
        if (qtyInItemUnit.IsFailure)
        {
            return Result.Failure<Guid>(qtyInItemUnit.Error);
        }
        foodItem.AdjustReservedQuantity(
            qtyInItemUnit.Value,
            userContext.UserId
        );

        reservation.MarkAsCancelled();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}

