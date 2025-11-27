using Pento.Application.Abstractions.Messaging;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.GetCurrentSubscriptions;

public sealed record GetCurrentSubscriptionsQuery(
    string? SearchText,
    SubscriptionStatus? Status, 
    int? FromDuration, 
    int? ToDuration) : IQuery<IReadOnlyList<UserSubscriptionResponse>>;
