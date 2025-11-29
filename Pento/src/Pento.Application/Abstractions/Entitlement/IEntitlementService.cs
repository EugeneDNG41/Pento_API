

using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Abstractions.Entitlement;

public interface IEntitlementService
{
    Task<Result> CheckEntitlementAsync(Guid userId, string featureCode, CancellationToken cancellationToken = default);
}
