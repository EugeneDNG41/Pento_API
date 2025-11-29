using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GiveawayClaims;

public static class GiveawayClaimErrors
{
    public static readonly Error PostNotFound = Error.NotFound(
        code: "GiveawayClaim.PostNotFound",
        description: "The specified giveaway post was not found.");

    public static readonly Error CannotClaimOwnPost = Error.Failure(
        code: "GiveawayClaim.CannotClaimOwnPost",
        description: "You cannot claim your own giveaway post.");
    public static readonly Error PostAlreadyClaimed = Error.Failure(
        code: "GiveawayClaim.PostAlreadyClaimed",
        description: "This giveaway post has already been claimed.");
    public static readonly Error PostExpired = Error.Failure(
        code: "GiveawayClaim.PostExpired",
        description: "This giveaway post has expired and can no longer be claimed.");
    public static readonly Error DuplicatePendingClaim = Error.Failure(
        code: "GiveawayClaim.DuplicatePendingClaim",
        description: "You already have a pending claim for this giveaway post.");


}
