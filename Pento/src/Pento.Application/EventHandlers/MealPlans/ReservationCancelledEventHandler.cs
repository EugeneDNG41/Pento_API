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

internal sealed class ReservationCancelledEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<FoodItemReservation> reservationRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<ReservationCancelledDomainEvent>
{
    public async override Task Handle(ReservationCancelledDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        FoodItemReservation? reservation = await reservationRepository.GetByIdAsync(domainEvent.ReservationId, cancellationToken);
        if (reservation == null)
        {
            throw new PentoException(nameof(ReservationCancelledEventHandler), CompartmentErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            reservation.HouseholdId,
            ActivityCode.MEAL_PLAN_CANCELLED.ToString(),
            domainEvent.ReservationId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(ReservationCancelledEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(ReservationCancelledEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
