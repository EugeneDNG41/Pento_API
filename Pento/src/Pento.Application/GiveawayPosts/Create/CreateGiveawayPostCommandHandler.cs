using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Giveaways.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Create;

internal sealed class CreateGiveawayPostCommandHandler(
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<GiveawayPost> giveawayRepo,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateGiveawayPostCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGiveawayPostCommand cmd, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        FoodItem? foodItem = await foodItemRepo.GetByIdAsync(cmd.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        if (cmd.Quantity > foodItem.Quantity)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.InsufficientQuantity);
        }

        if (cmd.PickupStartDate.HasValue && cmd.PickupEndDate.HasValue && cmd.PickupStartDate > cmd.PickupEndDate)
        {
            return Result.Failure<Guid>(GiveawayPostErrors.InvalidDateRange);
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


        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(post.Id);
    }
}
