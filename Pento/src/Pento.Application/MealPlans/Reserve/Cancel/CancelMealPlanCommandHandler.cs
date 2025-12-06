using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve.Cancel;
internal sealed class CancelMealPlanCommandHandler(
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

            item.AdjustQuantity(
                item.Quantity + r.Quantity,
                userContext.UserId
            );

            r.MarkAsCancelled();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return command.MealPlanId;
    }
}

