using Pento.Domain.Abstractions;

namespace Pento.Domain.GiveawayClaims;

public sealed class GiveawayClaim : Entity
{
    public GiveawayClaim(
       Guid id,
       Guid giveawayPostId,
       Guid claimantId,
       ClaimStatus status,
       string? message,
       DateTime createdOnUtc)
       : base(id)
    {
        GiveawayPostId = giveawayPostId;
        ClaimantId = claimantId;
        Status = status;
        Message = message;
        CreatedOnUtc = createdOnUtc;
    }


    private GiveawayClaim() { }

    public Guid GiveawayPostId { get; private set; }

    public Guid ClaimantId { get; private set; }

    public ClaimStatus Status { get; private set; }

    public string? Message { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }
    public void UpdateStatus(ClaimStatus newStatus)
    {
        Status = newStatus;
    }

}
