using Pento.Domain.Abstractions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Abstractions.DomainServices;

public interface ISubscriptionService
{
    Task<Result> ActivateAsync(UserSubscription userSubscription, CancellationToken cancellationToken);
    Task<Result> DeactivateAsync(UserSubscription userSubscription, CancellationToken cancellationToken);
}
