using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Application.Abstractions.UtilityServices.Converter;
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

        MealPlan? mealPlan = (await mealPlanRepository.FindAsync(
            x => x.HouseholdId == householdId.Value
              && x.MealType == command.MealType
              && x.ScheduledDate == command.ScheduledDate,
            cancellationToken
        )).FirstOrDefault();

        if (mealPlan is null)
        {
            string defaultName =
                $"{command.MealType}-{command.ScheduledDate:yyyy-MM-dd}";

            mealPlan = MealPlan.Create(
                householdId: householdId.Value,
                name: defaultName,
                mealType: command.MealType,
                scheduledDate: command.ScheduledDate,
                servings: command.Servings ?? 1,
                notes: null,
                createdBy: userContext.UserId,
                utcNow: dateTimeProvider.UtcNow
            );

            mealPlanRepository.Add(mealPlan);
        }

        decimal qtyInItemUnit = command.Quantity;

        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> converted = await converter.ConvertAsync(
                command.Quantity,
                command.UnitId,
                foodItem.UnitId,
                cancellationToken
            );

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

        var reservation = FoodItemMealPlanReservation.Create(
            foodItemId: foodItem.Id,
            householdId: householdId.Value,
            reservationDateUtc: dateTimeProvider.UtcNow,
            quantity: qtyInItemUnit,
            unitId: foodItem.UnitId,
            mealPlanId: mealPlan.Id
        );

        reservationRepository.Add(reservation);

        foodItem.Reserve(
            qtyInItemUnit,
            command.Quantity,
            command.UnitId,
            userContext.UserId
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}

