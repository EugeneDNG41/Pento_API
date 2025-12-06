using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.Households;

namespace Pento.Application.GiveawayPosts.Create;

internal sealed class CreateGiveawayPostCommandHandler(
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<GiveawayPost> giveawayRepo,
    IGenericRepository<FoodItemDonationReservation> donationReservationRepo,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateGiveawayPostCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGiveawayPostCommand cmd, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        FoodItem? foodItem = await foodItemRepo.GetByIdAsync(cmd.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }



        decimal qtyInItemUnit = cmd.Quantity;


        if (qtyInItemUnit > foodItem.Quantity)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.InsufficientQuantity);
        }


        var post = new GiveawayPost(
            id: Guid.CreateVersion7(),
            userId: userId,
            foodItemId: cmd.FoodItemId,
            titleDescription: cmd.TitleDescription,
            contactInfo: cmd.ContactInfo,
            status: GiveawayStatus.Open,
            pickupStartDate: cmd.PickupStartDate,
            pickupEndDate: cmd.PickupEndDate,
            pickupOption: cmd.PickupOption,
            createdOnUtc: clock.UtcNow,
            address: cmd.Address,
            quantity: cmd.Quantity
        );

        giveawayRepo.Add(post);

        foodItem.Reserve(
            qtyInItemUnit,          
            cmd.Quantity,           
            foodItem.UnitId,        
            userId
        );

        var donationReservation = new FoodItemDonationReservation(
            id: Guid.CreateVersion7(),
            foodItemId: foodItem.Id,
            householdId: householdId.Value,
            reservationDateUtc: clock.UtcNow,
            quantity: qtyInItemUnit,
            unitId: foodItem.UnitId,
            reservationStatus: ReservationStatus.Pending,
            reservationFor: ReservationFor.Donation,
            giveAwayPostId: post.Id
        );

        donationReservationRepo.Add(donationReservation);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(post.Id);
    }
}
