using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.GetById;

public sealed record GetSubscriptionByIdQuery(Guid SubscriptionId) : IQuery<SubscriptionDetailResponse>;
