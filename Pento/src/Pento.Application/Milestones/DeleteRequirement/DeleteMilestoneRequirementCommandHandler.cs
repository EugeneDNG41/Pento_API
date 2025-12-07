using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.DeleteRequirement;

internal sealed class DeleteMilestoneRequirementCommandHandler(
    IGenericRepository<MilestoneRequirement> milestoneRequirementRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteMilestoneRequirementCommand>
{
    public async Task<Result> Handle(DeleteMilestoneRequirementCommand command, CancellationToken cancellationToken)
    {
        MilestoneRequirement? milestoneRequirement = await milestoneRequirementRepository.GetByIdAsync(command.Id, cancellationToken);
        if (milestoneRequirement is null)
        {
            return Result.Failure(MilestoneErrors.RequirementNotFound);
        }
        milestoneRequirementRepository.Remove(milestoneRequirement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
