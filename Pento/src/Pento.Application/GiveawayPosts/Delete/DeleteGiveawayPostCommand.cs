using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GiveawayPosts.Delete;

public sealed record DeleteGiveawayPostCommand(Guid Id) : ICommand<Guid>;

