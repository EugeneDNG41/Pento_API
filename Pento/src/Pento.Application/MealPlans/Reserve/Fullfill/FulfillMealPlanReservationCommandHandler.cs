using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
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

        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(
            reservation.FoodItemId,
            cancellationToken);

        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
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

        if (qtyFulfillInItemUnit > foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }

        (FoodItemReservation fulfilledReservation, _) = reservation.FulfillPartially(
            qtyFulfillInItemUnit,
            foodItem.UnitId,
            userContext.UserId);

        foodItem.AdjustQuantity(foodItem.Quantity - qtyFulfillInItemUnit, userContext.UserId);

        if (fulfilledReservation is not null && fulfilledReservation.Id != reservation.Id)
        {
            reservationRepository.Add((FoodItemMealPlanReservation)fulfilledReservation);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}

