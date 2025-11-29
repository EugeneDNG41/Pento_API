using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.GetCurrentSubscriptionById;

public sealed record GetCurrentSubscriptionByIdQuery(Guid UserSubscriptionId) : IQuery<UserSubscriptionDetailResponse>;
