using Pento.Domain.Users;

namespace Pento.Infrastructure.External.Identity;

internal sealed record UserRepresentation(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    bool EmailVerified,
    bool Enabled,
    CredentialRepresentation[] Credentials)
{
    public UserRepresentation() : this(
        Username: string.Empty,
        Email: string.Empty,
        FirstName: string.Empty,
        LastName: string.Empty,
        EmailVerified: false,
        Enabled: false,
        Credentials: Array.Empty<CredentialRepresentation>())
    {
    }
    internal static UserRepresentation FromUser(User user) =>
        new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Email,
            Enabled = true,
            EmailVerified = true
        };
}
