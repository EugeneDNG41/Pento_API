using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.AddFeature;

public sealed record AddSubscriptionFeatureCommand(
    Guid SubscriptionId,
    string FeatureCode,
    int? Quota,
    TimeUnit? ResetPeriod) : ICommand<Guid>;


