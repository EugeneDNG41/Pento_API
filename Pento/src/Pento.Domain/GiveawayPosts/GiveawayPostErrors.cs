using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GiveawayPosts;

public static class GiveawayPostErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "GiveawayPost.NotFound",
        "The specified giveaway post was not found."
    );
    public static readonly Error InvalidDateRange = Error.Conflict(
        "GiveawayPost.InvalidDateRange",
        "The pickup end date must be later than the pickup start date."
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
}
