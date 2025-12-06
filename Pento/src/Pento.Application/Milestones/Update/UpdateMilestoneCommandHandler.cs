using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.Update;

internal sealed class UpdateMilestoneCommandHandler(
    IGenericRepository<Milestone> milestoneRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateMilestoneCommand>
{
    public async Task<Result> Handle(UpdateMilestoneCommand command, CancellationToken cancellationToken)
    {
        Milestone? milestone = await milestoneRepository.GetByIdAsync(command.Id, cancellationToken);
        if (milestone is null)
        {
            return Result.Failure(MilestoneErrors.NotFound);
        }
        if (command.Name != null && milestone.Name != command.Name)
        {
            bool nameTaken = await milestoneRepository
                .AnyAsync(s => s.Name == command.Name && s.Id != command.Id, cancellationToken);
            if (nameTaken)
            {
                return Result.Failure(MilestoneErrors.NameTaken);
            }
        }
        milestone.UpdateDetails(command.Name, command.Description, command.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
