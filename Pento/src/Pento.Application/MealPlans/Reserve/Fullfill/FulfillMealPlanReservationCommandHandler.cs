using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
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

        FoodItemMealPlanReservation? reservation = await reservationRepository.GetByIdAsync(
            command.ReservationId,
            cancellationToken);

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

        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(
            reservation.FoodItemId,
            cancellationToken);

        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        decimal qtyFulfilledInItemUnit = command.NewQuantity;
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

            qtyFulfilledInItemUnit = converted.Value;
        }

        decimal reservedQty = reservation.Quantity;

        if (qtyFulfilledInItemUnit < reservedQty)
        {
            decimal delta = reservedQty - qtyFulfilledInItemUnit;
            foodItem.AdjustQuantity(foodItem.Quantity + delta, userContext.UserId);
        }

        else if (qtyFulfilledInItemUnit > reservedQty)
        {
            decimal delta = qtyFulfilledInItemUnit - reservedQty;

            if (delta > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }

            foodItem.AdjustQuantity(foodItem.Quantity - delta, userContext.UserId);
        }

        reservation.MarkAsFulfilled(command.NewQuantity, command.UnitId, userContext.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;

    }
}
