using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Households;

public sealed class Household : Entity
{
    public string Name { get; private set; }
    public string? InviteCode { get; private set; }
    public DateTime? InviteCodeExpirationUtc { get; private set; }

    private Household() { }
    public Household(string name)
    {
        Name = name;
    }
    public static Household Create(string name)
    {
        return new Household(name);
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
}
