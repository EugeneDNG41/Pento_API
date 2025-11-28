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
}
