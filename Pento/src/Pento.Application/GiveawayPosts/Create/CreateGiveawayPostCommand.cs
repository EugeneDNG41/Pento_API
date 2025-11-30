using Pento.Application.Abstractions.Messaging;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Create;

public sealed record CreateGiveawayPostCommand(
    Guid FoodItemId,
    string TitleDescription,
    string ContactInfo,
    DateTime? PickupStartDate,
    DateTime? PickupEndDate,
    PickupOption PickupOption,
    string Address,
    decimal Quantity
) : ICommand<Guid>;
