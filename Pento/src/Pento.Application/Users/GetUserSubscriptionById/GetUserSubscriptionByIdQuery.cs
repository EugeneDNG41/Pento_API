using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentSubscriptionById;

namespace Pento.Application.Users.GetUserSubscriptionById;

public sealed record GetUserSubscriptionByIdQuery(Guid UserSubscriptionId) : IQuery<UserSubscriptionDetailResponse>;
