using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;
using Pento.Domain.UserMilestones;

namespace Pento.Application.EventHandlers;

internal sealed class UserMilestoneCreatedEventHandler(
    INotificationService notificationService,
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository) : DomainEventHandler<UserMilestoneCreatedDomainEvent>
{
    public async override Task Handle(UserMilestoneCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        UserMilestone? userMilestone = await userMilestoneRepository.GetByIdAsync(domainEvent.UserMilestoneId, cancellationToken);
        if (userMilestone == null)
        {
            throw new PentoException(nameof(UserMilestoneCreatedEventHandler), MilestoneErrors.UserMilestoneNotFound);
        }
        Milestone? milestone = await milestoneRepository.GetByIdAsync(userMilestone.MilestoneId, cancellationToken);
        if (milestone == null)
        {
            throw new PentoException(nameof(UserMilestoneCreatedEventHandler), MilestoneErrors.NotFound);
        }
        Result notificationResult =
            await notificationService.SendUserMilestoneAchievedAsync(userMilestone.UserId, milestone.Id, milestone.Name, cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(UserMilestoneCreatedEventHandler), notificationResult.Error);
        }
    }
}
