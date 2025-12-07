using Pento.Domain.Shared;

namespace Pento.Application.UserSubscriptions.AddUserSubscription;

public sealed record AddUserSubscriptionCommand(Guid UserId, Guid SubscriptionId, int? DurationInDays);
// POST admin/users/{userId}/subscriptions
public sealed record AddUserEntitlementCommand(Guid UserId, string FeatureCode, Guid UserSubscriptionId, int? Quota, TimeUnit? ResetPeriod);
// POST admin/users/{userId}/entitlements
public sealed record DeleteUserEntitlementCommand(Guid UserEntitlementId);
// DELETE admin/users/entitlements/{userEntitlementId}
