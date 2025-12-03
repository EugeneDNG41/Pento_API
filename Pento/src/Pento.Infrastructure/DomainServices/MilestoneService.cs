using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.DomainServices;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;
using Pento.Domain.Shared;
using Pento.Domain.UserActivities;
using Pento.Domain.UserMilestones;

namespace Pento.Infrastructure.DomainServices;
internal sealed class MilestoneService(
    IDateTimeProvider dateTimeProvider, //need notification service later
    IActivityService activityService,
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<MilestoneRequirement> milestoneRequirementRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository,   
    IUnitOfWork unitOfWork)
    : IMilestoneService
{
    public async Task<Result> CheckMilestoneAsync(UserActivity userActivity, CancellationToken cancellationToken)
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
                DateTime? fromDate = requirement.WithinDays.HasValue
                    ? dateTimeProvider.UtcNow.AddDays(-requirement.WithinDays.Value)
                    : null;
                Result<int> countResult = await activityService.CountActivityAsync(userActivity.UserId, requirement.ActivityCode, fromDate, cancellationToken);
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
        Result result = await CheckMilestoneAsync(userActivity, cancellationToken);
        if (result.IsSuccess)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);          
        }
        return result;
    }
}
