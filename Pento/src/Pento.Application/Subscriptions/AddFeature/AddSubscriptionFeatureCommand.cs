using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.AddFeature;

public sealed record AddSubscriptionFeatureCommand(
    Guid SubscriptionId,
    string FeatureName,
    int? EntitlementQuota,
    TimeUnit? EntitlementResetPer) : ICommand<Guid>;


