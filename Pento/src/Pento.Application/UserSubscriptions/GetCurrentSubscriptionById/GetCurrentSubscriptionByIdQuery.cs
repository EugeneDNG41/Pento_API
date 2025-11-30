using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.GetCurrentSubscriptionById;

public sealed record GetCurrentSubscriptionByIdQuery(Guid UserSubscriptionId) : IQuery<UserSubscriptionDetailResponse>;
