using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Delete;

internal sealed class DeleteGiveawayPostCommandHandler(
    IGenericRepository<GiveawayPost> giveawayRepo,
    IUnitOfWork uow,
    IDateTimeProvider clock
) : ICommandHandler<DeleteGiveawayPostCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteGiveawayPostCommand cmd, CancellationToken cancellationToken)
    {
        GiveawayPost? post = await giveawayRepo.GetByIdAsync(cmd.Id, cancellationToken);
        if (post is null)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.NotFound);
        }

        if (post.Status is GiveawayStatus.Claimed or GiveawayStatus.Expired)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.CannotDelete);
        }

        post.MarkAsDeleted(clock.UtcNow);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(cmd.Id);
    }
}
