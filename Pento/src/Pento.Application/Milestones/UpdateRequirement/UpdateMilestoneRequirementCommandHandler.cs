using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.UpdateRequirement;

internal sealed class UpdateMilestoneRequirementCommandHandler(
    IGenericRepository<MilestoneRequirement> milestoneRequirementRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateMilestoneRequirementCommand>
{
    public async Task<Result> Handle(UpdateMilestoneRequirementCommand command, CancellationToken cancellationToken)
    {
        MilestoneRequirement? milestoneRequirement = await milestoneRequirementRepository.GetByIdAsync(command.Id, cancellationToken);
        if (milestoneRequirement is null)
        {
            return Result.Failure(MilestoneErrors.RequirementNotFound);
        }
        if (command.ActivityCode != null && milestoneRequirement.ActivityCode != command.ActivityCode)
        {
            bool requirementExists = await milestoneRequirementRepository
                .AnyAsync(mr => mr.Id != command.Id 
                && mr.MilestoneId == milestoneRequirement.MilestoneId 
                && mr.ActivityCode == command.ActivityCode, cancellationToken);
            if (requirementExists)
            {
                return Result.Failure(MilestoneErrors.DuplicateRequirement);
            }
        }        
        milestoneRequirement.UpdateDetails(command.ActivityCode, command.Quota, command.WithinDays);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
