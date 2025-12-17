using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.DeletePlan;

internal sealed class DeleteSubscriptionPlanCommandHandler(
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteSubscriptionPlanCommand>
{
    public async Task<Result> Handle(DeleteSubscriptionPlanCommand command, CancellationToken cancellationToken)
    {
        SubscriptionPlan? subscriptionPlan = await subscriptionPlanRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscriptionPlan is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionPlanNotFound);
        }
        await subscriptionPlanRepository.RemoveAsync(subscriptionPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
