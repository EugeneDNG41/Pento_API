using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.DomainServices;

public interface IEntitlementService
{
    Task<Result> UseEntitlementAsync(Guid userId, string featureCode, CancellationToken cancellationToken);
}
