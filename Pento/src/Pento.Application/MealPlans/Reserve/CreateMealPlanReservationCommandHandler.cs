using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Reserve;

internal sealed class CreateMealPlanReservationCommandHandler(
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<MealPlan> mealPlanRepository,
    IGenericRepository<FoodItemMealPlanReservation> reservationRepository,
    IConverterService converter,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMealPlanReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateMealPlanReservationCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        if (foodItem.HouseholdId != householdId.Value)
        {
            return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
        }

        MealPlan? mealPlan = await mealPlanRepository.GetByIdAsync(command.MealPlanId, cancellationToken);
        if (mealPlan is null)
        {
            return Result.Failure<Guid>(MealPlanErrors.NotFound);
        }

        if (mealPlan.HouseholdId != householdId.Value)
        {
            return Result.Failure<Guid>(MealPlanErrors.ForbiddenAccess);
        }

        decimal requestedQtyInItemUnit = command.Quantity;

        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> convertedResult = await converter.ConvertAsync(
                command.Quantity,
                fromUnitId: command.UnitId,
                toUnitId: foodItem.UnitId,
                cancellationToken);

            if (convertedResult.IsFailure)
            {
                return Result.Failure<Guid>(convertedResult.Error);
            }

            requestedQtyInItemUnit = convertedResult.Value;
        }

        if (requestedQtyInItemUnit > foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }

        var reservation = FoodItemMealPlanReservation.Create(
            foodItemId: command.FoodItemId,
            householdId: householdId.Value,
            reservationDateUtc: dateTimeProvider.UtcNow,
            quantity: requestedQtyInItemUnit,
            unitId: command.UnitId,
            mealPlanId: command.MealPlanId
        );

        foodItem.Reserve(requestedQtyInItemUnit, command.Quantity, command.UnitId, userContext.UserId);

        reservationRepository.Add(reservation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
