using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.DomainServices;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.PauseSubscription;

internal sealed class PauseSubscriptionCommandHandler(
    IDateTimeProvider dateTimeProvider,
    ISubscriptionService subscriptionService,
    IUserContext userContext,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<PauseSubscriptionCommand>
{
    public async Task<Result> Handle(PauseSubscriptionCommand request, CancellationToken cancellationToken)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(request.UserSubscriptionId, cancellationToken);
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
        else if (userSubscription.Status == SubscriptionStatus.Paused)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionPaused);
        }
        else if (!userSubscription.EndDate.HasValue)
        {
            return Result.Failure(SubscriptionErrors.CannotPauseLifetimeSubscription);
        }
        else
        {
            userSubscription.Pause(dateTimeProvider.Today);
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

