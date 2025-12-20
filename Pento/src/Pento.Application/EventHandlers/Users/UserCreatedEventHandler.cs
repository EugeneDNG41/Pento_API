using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Features;
using Pento.Domain.UserEntitlements;
using Pento.Domain.Users.Events;

namespace Pento.Application.EventHandlers.Users;

internal sealed class UserCreatedEventHandler(
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<UserCreatedDomainEvent>
{
    public override async Task Handle(
        UserCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Feature> features = await featureRepository.FindAsync(f => f.DefaultQuota != null, cancellationToken);
        if (features.Any())
        {
            var entitlementList = new List<UserEntitlement>();
            foreach (Feature feature in features)
            {
                var userEntitlement = UserEntitlement.Create(
                    domainEvent.UserId,
                    null,
                    feature.Code,
                    feature.DefaultQuota,
                    feature.DefaultResetPeriod);
                entitlementList.Add(userEntitlement);
            }
            userEntitlementRepository.AddRange(entitlementList);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
