using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.GetCurrentSubscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.GetUserSubscriptions;

public sealed record GetUserSubscriptionsQuery(
    Guid UserId,
    string? SearchText,
    SubscriptionStatus? Status,
    int? FromDuration,
    int? ToDuration) : IQuery<IReadOnlyList<UserSubscriptionResponse>>;
