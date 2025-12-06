using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Services;

public interface IEntitlementService
{
    Task<Result> UseEntitlementAsync(Guid userId, string featureCode, CancellationToken cancellationToken);
}
