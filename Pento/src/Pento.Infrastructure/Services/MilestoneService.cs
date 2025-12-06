using System.Threading;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;
using Pento.Domain.Shared;
using Pento.Domain.UserActivities;
using Pento.Domain.UserMilestones;

namespace Pento.Infrastructure.Services;
internal sealed class MilestoneService(
    IDateTimeProvider dateTimeProvider, //need notification service later
    IActivityService activityService,
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<MilestoneRequirement> milestoneRequirementRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository,   
    IUnitOfWork unitOfWork)
    : IMilestoneService
{
    public async Task<Result> CheckMilestoneAfterActivityAsync(UserActivity userActivity, CancellationToken cancellationToken)
    {

        IEnumerable<MilestoneRequirement> relatedRequirements = await milestoneRequirementRepository.FindAsync(
            mr => mr.ActivityCode == userActivity.ActivityCode,
            cancellationToken);
        IEnumerable<Guid> milestoneIds = relatedRequirements.Select(rr => rr.MilestoneId).Distinct();
        foreach (Guid milestoneId in milestoneIds)
        {
            Milestone? milestone = await milestoneRepository.GetByIdAsync(milestoneId, cancellationToken);
            if (milestone is null)
            {
                return Result.Failure(MilestoneErrors.NotFound);
            }
            if (!milestone.IsActive)
            {
                continue;
            }
            UserMilestone? userMilestone = (await userMilestoneRepository.FindAsync(
                um => um.UserId == userActivity.UserId && um.MilestoneId == milestoneId,
                cancellationToken)).SingleOrDefault();
            if (userMilestone is not null)
            {
                continue;
            }
            IEnumerable<MilestoneRequirement> requirementsForMilestone = relatedRequirements
                .Where(rr => rr.MilestoneId == milestoneId);
            bool requirementsMet = true;
            foreach (MilestoneRequirement requirement in requirementsForMilestone)
            {
                Result<int> countResult = requirement.WithinDays.HasValue ? 
                    await activityService.CountMostWithinTimeAsync(userActivity.UserId, requirement.ActivityCode, TimeSpan.FromDays(requirement.WithinDays.Value), cancellationToken) :
                    await activityService.CountActivityAsync(userActivity.UserId, requirement.ActivityCode, cancellationToken); // replace with count against quota
                if (countResult.IsFailure)
                {
                    return Result.Failure(countResult.Error);
                }
                if (countResult.Value < requirement.Quota)
                {
                    requirementsMet = false;
                }
            }         
            if (userMilestone is null && requirementsMet)
            {
                var newUserMilestone = UserMilestone.Create(userActivity.UserId, milestoneId, dateTimeProvider.UtcNow);
                userMilestoneRepository.Add(newUserMilestone);
            }
        }
        return Result.Success();
    }
    public async Task<Result> CheckMilestoneWithSaveChangesAsync(UserActivity userActivity, CancellationToken cancellationToken)
    {
        Result result = await CheckMilestoneAfterActivityAsync(userActivity, cancellationToken);
        if (result.IsSuccess)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);          
        }
        return result;
    }
}
