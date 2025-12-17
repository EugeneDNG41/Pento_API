using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve.Fullfill;

internal sealed class FulfillMealPlanReservationCommandHandler(
    IGenericRepository<FoodItemMealPlanReservation> reservationRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IConverterService converter,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<FulfillMealPlanReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
    FulfillMealPlanReservationCommand command,
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
        decimal qtyFulfillInReservedUnit = command.NewQuantity;
        if (reservation.UnitId != command.UnitId)
        {
            Result<decimal> converted = await converter.ConvertAsync(
                command.NewQuantity,
                command.UnitId,
                reservation.UnitId,
                cancellationToken);

            if (converted.IsFailure)
            {
                return Result.Failure<Guid>(converted.Error);
            }

            qtyFulfillInReservedUnit = converted.Value;
        }
        decimal qtyFulfillInItemUnit = command.NewQuantity;
        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> converted = await converter.ConvertAsync(
                command.NewQuantity,
                command.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (converted.IsFailure)
            {
                return Result.Failure<Guid>(converted.Error);
            }
            qtyFulfillInItemUnit = converted.Value;
        }
        decimal qtyReservedInItemUnit = reservation.Quantity;
        if (reservation.UnitId != foodItem.UnitId)
        {
            Result<decimal> converted = await converter.ConvertAsync(
                reservation.Quantity,
                reservation.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (converted.IsFailure)
            {
                return Result.Failure<Guid>(converted.Error);
            }
            qtyReservedInItemUnit = converted.Value;
        }
        if (qtyFulfillInReservedUnit < reservation.Quantity)
        {
            var leftover = FoodItemMealPlanReservation.Create(
                reservation.FoodItemId,
                reservation.HouseholdId,
                reservation.ReservationDateUtc,
                reservation.Quantity - qtyFulfillInReservedUnit,
                reservation.UnitId,
                reservation.MealPlanId);
            reservationRepository.Add(leftover);
        }
        if (qtyFulfillInItemUnit - qtyReservedInItemUnit > foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }
        reservation.MarkAsFulfilled(command.NewQuantity, command.UnitId, userContext.UserId);
        await reservationRepository.UpdateAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}

