using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve.Fullfill;

internal sealed class FulfillMealPlanCommandHandler(
    IGenericRepository<FoodItemMealPlanReservation> reservationRepo,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<FulfillMealPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        FulfillMealPlanCommand command,
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

        foreach (FoodItemMealPlanReservation? r in reservations)
        {
            r.MarkAsFulfilled(
                r.Quantity,
                r.UnitId,
                userContext.UserId
            );
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return command.MealPlanId;
    }
}
