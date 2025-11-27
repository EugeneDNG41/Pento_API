using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentSubscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.GetUserSubscriptions;

public sealed record GetUserSubscriptionsQuery(
    Guid UserId,
    string? SearchText,
    SubscriptionStatus? Status, 
    int? FromDuration, 
    int? ToDuration) : IQuery<IReadOnlyList<UserSubscriptionResponse>>;
