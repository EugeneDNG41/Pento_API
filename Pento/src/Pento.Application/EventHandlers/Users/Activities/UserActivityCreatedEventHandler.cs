using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.Users.Activities;

internal sealed class UserActivityCreatedEventHandler(
    IMilestoneService milestoneService,
    IGenericRepository<UserActivity> userActivityRepository)
    : DomainEventHandler<UserActivityCreatedDomainEvent>
{
    public override async Task Handle(
        UserActivityCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        UserActivity? activity = await userActivityRepository.GetByIdAsync(domainEvent.UserActivityId, cancellationToken);
        if (activity == null) 
        {
            throw new PentoException(nameof(UserActivityCreatedEventHandler), ActivityErrors.UserActivityNotFound);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneWithSaveChangesAsync(activity, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(UserActivityCreatedEventHandler), milestoneCheckResult.Error);
        }
    }
}
