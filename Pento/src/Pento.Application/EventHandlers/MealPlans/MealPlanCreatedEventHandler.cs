using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.MealPlans;
using Pento.Domain.MealPlans.Events;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.MealPlans;

internal sealed class MealPlanCreatedEventHandler(
    IActivityService activityService,
    IGenericRepository<MealPlan> mealPlanRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<MealPlanCreatedDomainEvent>
{
    public async override Task Handle(MealPlanCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetByIdAsync(domainEvent.MealPlanId, cancellationToken);
        if (mealPlan == null)
        {
            throw new PentoException(nameof(MealPlanCreatedEventHandler), MealPlanErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            mealPlan.HouseholdId,
            ActivityCode.MEAL_PLAN_CREATE.ToString(),
            domainEvent.MealPlanId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(MealPlanCreatedEventHandler), createResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
