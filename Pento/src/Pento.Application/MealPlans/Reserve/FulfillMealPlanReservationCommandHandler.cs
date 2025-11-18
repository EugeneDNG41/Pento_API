using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve;

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

        decimal qtyInItemUnit = command.NewQuantity;

        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> converted = await converter.ConvertAsync(
                command.NewQuantity,
                fromUnitId: command.UnitId,
                toUnitId: foodItem.UnitId,
                cancellationToken);

            if (converted.IsFailure)
            {
                return Result.Failure<Guid>(converted.Error);
            }

            qtyInItemUnit = converted.Value;
        }

        if (qtyInItemUnit > foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }

        foodItem.Reserve(
            qtyInItemUnit,
            command.NewQuantity,
            command.UnitId,
            userContext.UserId);

        reservation.UpdateQuantity(qtyInItemUnit);
        reservation.MarkAsFulfilled();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
