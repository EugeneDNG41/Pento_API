using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.DomainServices;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.CancelSubscription;

internal sealed class CancelSubscriptionCommandHandler(
    ISubscriptionService subscriptionService,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CancelSubscriptionCommand>
{
    public async Task<Result> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(command.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            return Result.Failure(SubscriptionErrors.UserSubscriptionNotFound);
        }
        if (userSubscription.UserId != userContext.UserId)
        {
            return Result.Failure(SubscriptionErrors.ForbiddenAccess);
        }
        if (userSubscription.Status == SubscriptionStatus.Expired)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionExpired);
        }
        else if (userSubscription.Status == SubscriptionStatus.Cancelled)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionCancelled);
        }
        else
        {
            userSubscription.Cancel(dateTimeProvider.Today, command.Reason);
            userSubscriptionRepository.Update(userSubscription);
            Result deactivationResult = await subscriptionService.DeactivateAsync(userSubscription, cancellationToken);
            if (deactivationResult.IsFailure)
            {
                return Result.Failure(deactivationResult.Error);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

