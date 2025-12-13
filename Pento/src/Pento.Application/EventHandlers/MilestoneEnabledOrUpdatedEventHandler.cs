using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;
using Pento.Domain.UserMilestones;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers;

internal sealed class MilestoneEnabledOrUpdatedEventHandler( 
    IMilestoneService milestoneService,
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<MilestoneEnabledOrUpdatedDomainEvent>
{
    public async override Task Handle(MilestoneEnabledOrUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Milestone? milestone = await milestoneRepository.GetByIdAsync(domainEvent.MilestoneId, cancellationToken);
        if (milestone == null)
        {
            throw new PentoException(nameof(MilestoneEnabledOrUpdatedEventHandler), MilestoneErrors.NotFound);
        }
        if (!milestone.IsActive)
        {
            return;
        }
        var alreadyEarnedUserIds = (await userMilestoneRepository.FindAsync(
            um => um.MilestoneId == milestone.Id,
            cancellationToken)).Select(um => um.UserId).ToHashSet();
        var notEarnedUserIds = (await userRepository.FindAsync(
            u => !alreadyEarnedUserIds.Contains(u.Id),
            cancellationToken)).Select(u => u.Id).ToHashSet();
        foreach (Guid userId in notEarnedUserIds)
        {
            Result checkResult = await milestoneService.CheckUserMilestoneAsync(userId, milestone, cancellationToken);
            if (checkResult.IsFailure)
            {
                throw new PentoException(nameof(MilestoneEnabledOrUpdatedEventHandler), checkResult.Error);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
