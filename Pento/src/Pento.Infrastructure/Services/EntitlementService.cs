using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;
using Pento.Domain.Roles;
using Pento.Domain.UserEntitlements;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Services;

internal sealed class EntitlementService(
    IGenericRepository<User> userRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IGenericRepository<Feature> featureRepository,
    IUnitOfWork unitOfWork) : IEntitlementService
{
    public async Task<Result> UseEntitlementAsync(Guid userId, string featureCode, CancellationToken cancellationToken)
    {
        User? user = (await userRepository.FindIncludeAsync(u => u.Id == userId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.Roles.Any(r => r.Type == RoleType.Administrative))
        {
            return Result.Success();
        }
        IEnumerable<UserEntitlement> userEntitlements = await userEntitlementRepository.FindAsync(
            ue => ue.UserId == userId &&
            ue.FeatureCode == featureCode.ToString(),
            cancellationToken);
        if (userEntitlements.Any())
        {
            UserEntitlement? eligibleEntitlement = userEntitlements
                .Where(ue => ue.Quota == null || ue.UsageCount < ue.Quota)
                .OrderBy(ue => ue.UserSubscriptionId)
                .FirstOrDefault();
            if (eligibleEntitlement == null)
            {
                return Result.Failure(UserEntitlementErrors.QuotaExceeded);
            }
            else
            {
                eligibleEntitlement.IncrementUsage();
            }
        }
        else
        {
            Feature? feature = (await featureRepository.FindAsync(f => f.Code == featureCode, cancellationToken)).SingleOrDefault();
            if (feature is null)
            {
                return Result.Failure(FeatureErrors.NotFound);
            }
            if (!feature.DefaultQuota.HasValue)
            {
                return Result.Failure(UserEntitlementErrors.SubscriptionNeeded);
            }
            else
            {
                var newEntitlement = UserEntitlement.Create(
                    userId,
                    null,
                    feature.Code,
                    feature.DefaultQuota,
                    feature.DefaultResetPeriod);
                newEntitlement.IncrementUsage();
                userEntitlementRepository.Add(newEntitlement);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
