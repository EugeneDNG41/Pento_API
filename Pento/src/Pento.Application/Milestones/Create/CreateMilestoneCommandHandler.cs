using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.Create;

internal sealed class CreateMilestoneCommandHandler(
    IGenericRepository<Milestone> milestoneRepository, 
    IUnitOfWork unitOfWork) : ICommandHandler<CreateMilestoneCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMilestoneCommand command, CancellationToken cancellationToken)
    {
        bool nameTaken = await milestoneRepository
            .AnyAsync(m => m.Name == command.Name, cancellationToken);
        if (nameTaken)
        {
            return Result.Failure<Guid>(MilestoneErrors.NameTaken);
        }
        var milestone = Milestone.Create(command.Name, command.Description, command.IsActive);
        milestoneRepository.Add(milestone);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return milestone.Id;
    }
}
