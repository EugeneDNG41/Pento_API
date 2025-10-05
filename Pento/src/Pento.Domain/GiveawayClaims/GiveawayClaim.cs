using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.GiveawayPosts;

namespace Pento.Domain.GiveawayClaims;
public sealed class GiveawayClaim : Entity
{
    public  GiveawayClaim(
       Guid id,
       Guid giveawayId,
       Guid claimerUserId,
       ClaimStatus status,
       string? message,
       DateTime createdOnUtc)
       : base(id)
    {
        GiveawayId = giveawayId;
        ClaimerUserId = claimerUserId;
        Status = status;
        Message = message;
        CreatedOnUtc = createdOnUtc;
    }

    private GiveawayClaim() { }

    public Guid GiveawayId { get; private set; }

    public Guid ClaimerUserId { get; private set; }

    public ClaimStatus Status { get; private set; }

    public string? Message { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }
}
    

