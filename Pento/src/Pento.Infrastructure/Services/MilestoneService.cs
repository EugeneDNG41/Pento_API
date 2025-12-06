using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;
using Pento.Domain.UserActivities;
using Pento.Domain.UserMilestones;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Services;
internal sealed class MilestoneService(
    IDateTimeProvider dateTimeProvider,
    IActivityService activityService,
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<MilestoneRequirement> milestoneRequirementRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository,   
    IUnitOfWork unitOfWork)
    : IMilestoneService
{
    public async Task<Result> CheckUserMilestoneAsync(Guid userId, Milestone milestone, CancellationToken cancellationToken)
    {
        
        UserMilestone? userMilestone = (await userMilestoneRepository.FindAsync(
            um => um.UserId == userId && um.MilestoneId == milestone.Id,
            cancellationToken)).SingleOrDefault();
        if (userMilestone is not null)
        {
            return Result.Success();
        }
        IEnumerable<MilestoneRequirement> requirementsForMilestone = await milestoneRequirementRepository.FindAsync(
            mr => mr.MilestoneId == milestone.Id,
            cancellationToken);
        bool requirementsMet = true;
        foreach (MilestoneRequirement requirement in requirementsForMilestone)
        {
            Result<int> countResult = requirement.WithinDays.HasValue ?
                await activityService.CountMostWithinTimeAsync(userId, requirement.ActivityCode, TimeSpan.FromDays(requirement.WithinDays.Value), cancellationToken) :
                await activityService.CountActivityAsync(userId, requirement.ActivityCode, cancellationToken); // replace with count against quota
            if (countResult.IsFailure)
            {
                return Result.Failure(countResult.Error);
            }
            if (countResult.Value < requirement.Quota)
            {
                requirementsMet = false;
            }
        }
        if (requirementsMet)
        {
            var newUserMilestone = UserMilestone.Create(userId, milestone.Id, dateTimeProvider.UtcNow);
            userMilestoneRepository.Add(newUserMilestone);        
        }
        return Result.Success();
    }
    public async Task<Result> CheckMilestoneAfterActivityAsync(UserActivity userActivity, CancellationToken cancellationToken)
    {

        IEnumerable<MilestoneRequirement> relatedRequirements = await milestoneRequirementRepository.FindAsync(
            mr => mr.ActivityCode == userActivity.ActivityCode,
            cancellationToken);
        IEnumerable<Guid> milestoneIds = relatedRequirements.Select(rr => rr.MilestoneId).Distinct();
        foreach (Guid milestoneId in milestoneIds)
        {
            Milestone? milestone = await milestoneRepository.GetByIdAsync(milestoneId, cancellationToken);
            if (milestone == null)
            {
                return Result.Failure(MilestoneErrors.NotFound);
            }
            if (!milestone.IsActive)
            {
                continue;
            }
            Result result = await CheckUserMilestoneAsync(userActivity.UserId, milestone, cancellationToken);
            if (result.IsFailure)
            {
                return result;
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
