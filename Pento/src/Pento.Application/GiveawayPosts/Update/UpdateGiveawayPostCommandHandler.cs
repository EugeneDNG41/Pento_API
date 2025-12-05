using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Update;

internal sealed class UpdateGiveawayPostCommandHandler(
    IGenericRepository<GiveawayPost> giveawayRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<UpdateGiveawayPostCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateGiveawayPostCommand cmd, CancellationToken cancellationToken)
    {
        GiveawayPost? post = await giveawayRepo.GetByIdAsync(cmd.Id, cancellationToken);
        if (post is null)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.NotFound);
        }

        if (post.Status == GiveawayStatus.Fulfilled)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.AlreadyClaimed);
        }

        if (post.Status == GiveawayStatus.Expired)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.AlreadyExpired);
        }

        if (cmd.TitleDescription is not null)
        {
            post.UpdateTitle(cmd.TitleDescription);
        }

        if (cmd.ContactInfo is not null)
        {
            post.UpdateContact(cmd.ContactInfo);
        }

        if (cmd.Address is not null)
        {
            post.UpdateAddress(cmd.Address);
        }

        if (cmd.PickupOption.HasValue)
        {
            post.UpdatePickupOption(cmd.PickupOption.Value);
        }

        if (cmd.PickupStartDate.HasValue)
        {
            post.UpdatePickupStart(cmd.PickupStartDate);
        }

        if (cmd.PickupEndDate.HasValue)
        {
            post.UpdatePickupEnd(cmd.PickupEndDate);
        }

        if (cmd.Quantity.HasValue)
        {
            FoodItem? foodItem = await foodItemRepo.GetByIdAsync(post.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<Guid>(FoodItemErrors.NotFound);
            }

            if (cmd.Quantity.Value > foodItem.Quantity)
            {
                return Result.Failure<Guid>(GiveawayPostErrors.InsufficientQuantity);
            }

            post.UpdateQuantity(cmd.Quantity.Value);
        }

        post.UpdateStatus(cmd.Status, clock.UtcNow);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(cmd.Id);
    }
}
