using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve.Cancel;

internal sealed class CancelMealPlanCommandHandler(
    IConverterService converter,
    IGenericRepository<FoodItemMealPlanReservation> reservationRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CancelMealPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CancelMealPlanCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        var reservations = (await reservationRepo.FindAsync(
            x => x.MealPlanId == command.MealPlanId &&
                 x.HouseholdId == householdId.Value,
            cancellationToken)).ToList();

        if (!reservations.Any())
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.NotFound);
        }

        foreach (FoodItemMealPlanReservation? r in reservations)
        {
            if (r.Status != ReservationStatus.Pending)
            {
                return Result.Failure<Guid>(FoodItemReservationErrors.InvalidState);
            }
        }

        var foodItemIds = reservations.Select(r => r.FoodItemId).Distinct().ToList();

        var foodItems = (await foodItemRepo.FindAsync(
            x => foodItemIds.Contains(x.Id) &&
                 x.HouseholdId == householdId.Value,
            cancellationToken)).ToList();

        if (foodItems.Count != foodItemIds.Count)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        foreach (FoodItemMealPlanReservation? r in reservations)
        {
            FoodItem item = foodItems.Single(fi => fi.Id == r.FoodItemId);
            Result<decimal> qtyInItemUnit = await converter.ConvertAsync(
                r.Quantity,
                r.UnitId,
                item.UnitId,
                cancellationToken
            );
            if (qtyInItemUnit.IsFailure)
            {
                return Result.Failure<Guid>(qtyInItemUnit.Error);
            }
            item.AdjustReservedQuantity(
                qtyInItemUnit.Value
            );
            await foodItemRepo.UpdateAsync(item, cancellationToken);
            r.MarkAsCancelled();
        }
        await reservationRepo.UpdateRangeAsync(reservations, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return command.MealPlanId;
    }
}

