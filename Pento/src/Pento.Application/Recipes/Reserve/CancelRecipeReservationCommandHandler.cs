using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;

namespace Pento.Application.Recipes.Reserve;

internal sealed class CancelRecipeReservationCommandHandler(
    IConverterService converterService,
    IGenericRepository<FoodItemRecipeReservation> reservationRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CancelRecipeReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CancelRecipeReservationCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        FoodItemRecipeReservation? reservation = await reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
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

        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(reservation.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }
        decimal qtyInItemUnit = reservation.Quantity;
        if (foodItem.UnitId != reservation.UnitId)
        {
            Result<decimal> conversionResult = await converterService.ConvertAsync(
                reservation.Quantity,
                reservation.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (conversionResult.IsFailure)
            {
                return Result.Failure<Guid>(FoodItemErrors.InvalidMeasurementUnit);
            }
            qtyInItemUnit = conversionResult.Value;
        }
        foodItem.CancelReservation(qtyInItemUnit, reservation.Id);

        reservation.MarkAsCancelled(userContext.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
