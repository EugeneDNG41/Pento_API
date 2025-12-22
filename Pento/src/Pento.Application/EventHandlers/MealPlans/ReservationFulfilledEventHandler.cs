using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.MealPlans;

internal sealed class ReservationFulfilledEventHandler(
    IActivityService activityService,
    IGenericRepository<FoodItemReservation> reservationRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<ReservationFulfilledDomainEvent>
{
    public async override Task Handle(ReservationFulfilledDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        FoodItemReservation? reservation = await reservationRepository.GetByIdAsync(domainEvent.ReservationId, cancellationToken);
        if (reservation == null)
        {
            throw new PentoException(nameof(ReservationFulfilledEventHandler), CompartmentErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            reservation.HouseholdId,
            ActivityCode.MEAL_PLAN_FULFILLED.ToString(),
            domainEvent.ReservationId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(ReservationFulfilledEventHandler), createResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
