using System.Security.Cryptography;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Households;

public sealed class Household : Entity
{
    public string Name { get; private set; }
    public string? InviteCode { get; private set; }
    public DateTime? InviteCodeExpirationUtc { get; private set; }
    public DateTime CreatedOn { get; private set; }

    private Household() { }
    public Household(Guid id, string name, DateTime createdOn) : base(id)
    {
        Name = name;
        CreatedOn = createdOn;
    }
    public static Household Create(string name, DateTime createdOn, Guid userId)
    {
        var household = new Household(Guid.CreateVersion7(), name, createdOn);
        household.GenerateInviteCode();
        household.Raise(new HouseholdCreatedDomainEvent(household.Id, userId));
        return household;
    }
    public void SetInviteCodeExpiration(DateTime? expirationUtc)
    {
        InviteCodeExpirationUtc = expirationUtc;
    }
    public void Update(string name)
    {
        Name = name;
    }
    public void GenerateInviteCode()
    {
        string inviteCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8))
                            .Replace("+", "-")
                            .Replace("/", "_")
                            .TrimEnd('=');
        InviteCode = inviteCode;
    }
    public void RevokeInviteCode()
    {
        InviteCode = null;
        InviteCodeExpirationUtc = null;
    }
    public new void Delete()
    {
        base.Delete();
        Raise(new HouseholdDeletedDomainEvent(Id));

    }
}
