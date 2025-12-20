using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Households;
using Pento.Domain.UserActivities;
using Pento.Domain.Users;
using Pento.Domain.Users.Events;

namespace Pento.Application.EventHandlers.Households;

internal sealed class HouseholdJoinedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Household> householdRepository,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<UserHouseholdJoinedDomainEvent>
{
    public async override Task Handle(UserHouseholdJoinedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Household? household = await householdRepository.GetByIdAsync(domainEvent.HouseholdId, cancellationToken);
        if (household == null)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), HouseholdErrors.NotFound);
        }
        User? user = await userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        if (user == null)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), UserErrors.NotFound);
        }
        Result<UserActivity> joinResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            domainEvent.HouseholdId,
            ActivityCode.HOUSEHOLD_JOIN.ToString(),
            domainEvent.HouseholdId,
            cancellationToken);
        if (joinResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), joinResult.Error);
        }
        Result milestoneCheckJoinResult = await milestoneService.CheckMilestoneAfterActivityAsync(joinResult.Value, cancellationToken);
        if (milestoneCheckJoinResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), milestoneCheckJoinResult.Error);
        }
        IEnumerable<User> otherMembers = await userRepository.FindAsync(u => u.HouseholdId == domainEvent.HouseholdId && u.Id != domainEvent.UserId,
            cancellationToken: cancellationToken);
        foreach (User member in otherMembers)
        {
            Result<UserActivity> joinedResult = await activityService.RecordActivityAsync(
                member.Id,
                domainEvent.HouseholdId,
                ActivityCode.HOUSEHOLD_MEMBER_JOINED.ToString(),
                domainEvent.UserId,
                cancellationToken);
            if (joinedResult.IsFailure)
            {
                throw new PentoException(nameof(HouseholdJoinedEventHandler), joinedResult.Error);
            }
            Result milestoneCheckJoinedResult = await milestoneService.CheckMilestoneAfterActivityAsync(joinedResult.Value, cancellationToken);
            if (milestoneCheckJoinedResult.IsFailure)
            {
                throw new PentoException(nameof(HouseholdJoinedEventHandler), milestoneCheckJoinedResult.Error);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
