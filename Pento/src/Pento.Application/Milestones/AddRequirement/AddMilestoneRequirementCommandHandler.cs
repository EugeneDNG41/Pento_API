using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.AddRequirement;

internal sealed class AddMilestoneRequirementCommandHandler(
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<Activity> activityRepository,
    IGenericRepository<MilestoneRequirement> milestoneRequirementRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddMilestoneRequirementCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddMilestoneRequirementCommand command, CancellationToken cancellationToken)
    {
        Milestone? milestone = await milestoneRepository.GetByIdAsync(command.MilestoneId, cancellationToken);
        if (milestone is null)
        {
            return Result.Failure<Guid>(MilestoneErrors.NotFound);
        }
        Activity? activity = (await activityRepository.FindAsync(a => a.Code == command.ActivityCode, cancellationToken)).SingleOrDefault();
        if (activity is null)
        {
            return Result.Failure<Guid>(ActivityErrors.NotFound);
        }
        bool requirementExists = await milestoneRequirementRepository
            .AnyAsync(mr => mr.MilestoneId == command.MilestoneId && mr.ActivityCode == command.ActivityCode, cancellationToken);
        if (requirementExists)
        {
            return Result.Failure<Guid>(MilestoneErrors.DuplicateRequirement);
        }
        var milestoneRequirement = MilestoneRequirement.Create(
            command.MilestoneId,
            command.ActivityCode,
            command.Quota,
            command.WithinDays);
        milestoneRequirementRepository.Add(milestoneRequirement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return milestone.Id;
    }
}
