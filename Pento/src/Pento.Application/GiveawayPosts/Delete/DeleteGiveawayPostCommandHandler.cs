using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.Households;

namespace Pento.Application.GiveawayPosts.Delete;

internal sealed class DeleteGiveawayPostCommandHandler(
    IGenericRepository<GiveawayPost> giveawayRepo,
    IGenericRepository<FoodItemDonationReservation> reservationRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IUserContext userContext,
    IUnitOfWork uow
) : ICommandHandler<DeleteGiveawayPostCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteGiveawayPostCommand cmd, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        GiveawayPost? post = await giveawayRepo.GetByIdAsync(cmd.Id, cancellationToken);
        if (post is null)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.NotFound);
        }

        if (post.Status is GiveawayStatus.Fulfilled or GiveawayStatus.Expired)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.CannotDelete);
        }

        FoodItemDonationReservation? reservation =
            (await reservationRepo.FindAsync(
                x => x.GiveawayPostId == post.Id
                  && x.Status == ReservationStatus.Pending,
                cancellationToken
            )).FirstOrDefault();

        if (reservation is not null)
        {
            FoodItem? foodItem = await foodItemRepo.GetByIdAsync(reservation.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<Guid>(FoodItemErrors.NotFound);
            }

            foodItem.AdjustQuantity(
                foodItem.Quantity + reservation.Quantity,
                userContext.UserId
            );

            reservation.MarkAsCancelled();
        }

        post.Delete();

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(cmd.Id);
    }
}

