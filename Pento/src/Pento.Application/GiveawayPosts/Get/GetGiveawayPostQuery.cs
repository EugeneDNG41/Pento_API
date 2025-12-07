using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GiveawayPosts.Get;

public sealed record GetGiveawayPostByIdQuery(Guid Id)
    : IQuery<GiveawayPostDetailResponse>;
