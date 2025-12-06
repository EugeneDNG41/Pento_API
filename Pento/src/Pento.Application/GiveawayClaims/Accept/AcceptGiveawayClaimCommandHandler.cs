using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayClaims.Accept;
using Pento.Domain.Abstractions;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;
using Pento.Application.Abstractions.Utility.Clock;

namespace Pento.Application.GiveawayClaims.Accept;

internal sealed class AcceptGiveawayClaimCommandHandler(
    IGenericRepository<GiveawayClaim> claimRepo,
    IGenericRepository<GiveawayPost> postRepo,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<AcceptGiveawayClaimCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AcceptGiveawayClaimCommand cmd, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        GiveawayClaim? claim = await claimRepo.GetByIdAsync(cmd.ClaimId, cancellationToken);
        if (claim is null)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.NotFound);
        }

        GiveawayPost? post = await postRepo.GetByIdAsync(claim.GiveawayPostId, cancellationToken);
        if (post is null)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.NotFound);
        }

        if (post.UserId != userId)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.NotOwner);
        }

        if (post.Status is GiveawayStatus.Cancelled
            or GiveawayStatus.Expired
            or GiveawayStatus.Fulfilled)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.PostNotAcceptable);
        }

        if (claim.Status != ClaimStatus.Pending)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.AlreadyProcessed);
        }

        claim.UpdateStatus(ClaimStatus.Accepted);

        post.UpdateStatus(GiveawayStatus.Fulfilled, clock.UtcNow);

        IEnumerable<GiveawayClaim> otherClaims = await claimRepo.FindAsync(
            x => x.GiveawayPostId == post.Id
              && x.Id != claim.Id
              && x.Status == ClaimStatus.Pending,
            cancellationToken);

        foreach (GiveawayClaim c in otherClaims)
        {
            c.UpdateStatus(ClaimStatus.Declined);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(cmd.ClaimId);
    }
}
