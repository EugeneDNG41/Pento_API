using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Households;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers;

internal sealed class HouseholdCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<HouseholdCreatedDomainEvent>
{
    public async override Task Handle(HouseholdCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Household? household = await householdRepository.GetByIdAsync(domainEvent.HouseholdId, cancellationToken);
        if (household == null)
        {
            throw new PentoException(nameof(HouseholdCreatedEventHandler), HouseholdErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            domainEvent.HouseholdId,
            ActivityCode.HOUSEHOLD_CREATE.ToString(),
            domainEvent.HouseholdId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
