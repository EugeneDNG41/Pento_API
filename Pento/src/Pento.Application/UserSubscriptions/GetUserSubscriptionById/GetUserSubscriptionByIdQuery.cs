using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.GetCurrentSubscriptionById;

namespace Pento.Application.UserSubscriptions.GetUserSubscriptionById;

public sealed record GetUserSubscriptionByIdQuery(Guid UserSubscriptionId) : IQuery<UserSubscriptionDetailResponse>;
