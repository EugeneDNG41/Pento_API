using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayClaims.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.Giveaways.Claims.Create;

internal sealed class CreateGiveawayClaimCommandHandler(
    IGenericRepository<GiveawayPost> giveawayRepo,
    IGenericRepository<GiveawayClaim> claimRepo,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateGiveawayClaimCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGiveawayClaimCommand cmd, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        GiveawayPost? post = await giveawayRepo.GetByIdAsync(cmd.GiveawayPostId, cancellationToken);
        if (post is null)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.PostNotFound);
        }

        if (post.UserId == userId)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.CannotClaimOwnPost);
        }
        if (post.Status == GiveawayStatus.Claimed)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.PostAlreadyClaimed);
        }

        if (post.Status == GiveawayStatus.Expired)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.PostExpired);
        }

        if (post.Status == GiveawayStatus.Cancelled)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.PostAlreadyClaimed);
        }

        bool hasPending = await claimRepo.AnyAsync(
            x => x.GiveawayPostId == cmd.GiveawayPostId &&
                 x.ClaimantId == userId &&
                 x.Status == ClaimStatus.Pending,
            cancellationToken);

        if (hasPending)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.DuplicatePendingClaim);
        }

        var claim = new GiveawayClaim(
            id: Guid.CreateVersion7(),
            giveawayPostId: cmd.GiveawayPostId,
            claimantId: userId,
            status: ClaimStatus.Pending,
            message: cmd.Message,
            createdOnUtc: clock.UtcNow
        );

        claimRepo.Add(claim);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(claim.Id);
    }
}
