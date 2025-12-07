using Pento.Domain.Abstractions;

namespace Pento.Domain.GiveawayPosts;

public static class GiveawayPostErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "GiveawayPost.NotFound",
        "The specified giveaway post was not found."
    );
    public static readonly Error InsufficientQuantity = Error.Conflict(
        "GiveawayPost.InsufficientQuantity",
        "The giveaway quantity must be greater than zero."
    );
    public static readonly Error AlreadyClaimed = Error.Conflict(
    code: "GiveawayPost.AlreadyClaimed",
    description: "This item has already been claimed by another user."
);
    public static readonly Error AlreadyExpired = Error.Conflict(
        code: "GiveawayPost.AlreadyExpired",
        description: "This post has already expired and cannot be claimed."
    );
    public static readonly Error CannotDelete = Error.Conflict(
        code: "GiveawayPost.CannotDelete",
        description: "Only Open or Cancelled posts can be deleted."
    );
    public static readonly Error PostNotInClaimedState = Error.Conflict(
        code: "GiveawayPost.PostNotInClaimedState",
        description: "Only posts in Claimed state can be completed."
    );
}
