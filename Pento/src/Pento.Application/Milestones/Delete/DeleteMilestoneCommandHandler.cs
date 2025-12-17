using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;
using Pento.Domain.UserMilestones;

namespace Pento.Application.Milestones.Delete;

internal sealed class DeleteMilestoneCommandHandler(
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteMilestoneCommand>
{
    public async Task<Result> Handle(DeleteMilestoneCommand command, CancellationToken cancellationToken)
    {
        Milestone? milestone = await milestoneRepository.GetByIdAsync(command.Id, cancellationToken);
        if (milestone is null)
        {
            return Result.Failure(MilestoneErrors.NotFound);
        }
        bool hasUserMilestones = await userMilestoneRepository
            .AnyAsync(um => um.MilestoneId == command.Id, cancellationToken);
        if (hasUserMilestones)
        {
            return Result.Failure(MilestoneErrors.MilestoneInUse);
        }
        await milestoneRepository.RemoveAsync(milestone, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
