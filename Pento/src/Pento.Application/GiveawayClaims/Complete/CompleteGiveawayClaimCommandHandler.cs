using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.Households;
using Pento.Application.Abstractions.Utility.Clock;

namespace Pento.Application.GiveawayClaims.Complete;
internal sealed class CompleteGiveawayClaimCommandHandler(
    IGenericRepository<GiveawayClaim> claimRepo,
    IGenericRepository<GiveawayPost> postRepo,
    IGenericRepository<FoodItemDonationReservation> reservationRepo,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CompleteGiveawayClaimCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CompleteGiveawayClaimCommand cmd,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

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

        if (claim.Status != ClaimStatus.Accepted)
        {
            return Result.Failure<Guid>(GiveawayClaimErrors.NotAccepted);
        }

        if (post.Status != GiveawayStatus.Fulfilled)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.PostNotInClaimedState);
        }

        FoodItemDonationReservation? reservation = (await reservationRepo.FindAsync(
            x => x.GiveawayPostId == post.Id
              && x.FoodItemId == post.FoodItemId
              && x.Status == ReservationStatus.Pending,
            cancellationToken
        )).FirstOrDefault();

        if (reservation is null)
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.NotFound);
        }

        reservation.MarkAsFulfilled(
            reservation.Quantity,
            reservation.UnitId,
            userId
        );

        post.UpdateStatus(GiveawayStatus.Fulfilled, clock.UtcNow);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(cmd.ClaimId);
    }
}

