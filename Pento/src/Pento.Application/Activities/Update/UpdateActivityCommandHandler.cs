using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;

namespace Pento.Application.Activities.Update;

internal sealed class UpdateActivityCommandHandler(
    IGenericRepository<Activity> activityRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateActivityCommand>
{
    public async Task<Result> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        Activity? activity = await activityRepository.GetByIdAsync(command.Code, cancellationToken);
        if (activity is null)
        {
            return Result.Failure(ActivityErrors.NotFound);
        }
        if (command.Name == activity.Name)
        {
            return Result.Failure(ActivityErrors.NameTaken);
        }
        activity.UpdateDetails(command.Name, command.Description);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
