using System.Net;
using Microsoft.Extensions.Logging;
using Pento.Application.Abstractions.ThirdPartyServices.Identity;
using Pento.Domain.Abstractions;
namespace Pento.Infrastructure.ThirdPartyServices.Identity;

internal sealed class IdentityProviderService(KeyCloakClient keyCloakClient, ILogger<IdentityProviderService> logger)
    : IIdentityProviderService
{
    private const string PasswordCredentialType = "password";

    // POST /admin/realms/{realm}/users
    public async Task<Result> SendVerificationEmailAsync(string identityId, CancellationToken cancellationToken = default)
    {
        await keyCloakClient.SendVerificationEmailAsync(identityId, cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string identityId, string newPassword, CancellationToken cancellationToken = default)
    {
        var credentialRepresentation = new CredentialRepresentation(PasswordCredentialType, newPassword, false);
        await keyCloakClient.ChangePasswordAsync(identityId, credentialRepresentation, cancellationToken);
        return Result.Success();
    }
    public async Task<Result> SendResetPasswordEmailAsync(string identityId, CancellationToken cancellationToken = default)
    {
        await keyCloakClient.SendResetPasswordEmailAsync(identityId, cancellationToken);
        return Result.Success();
    }
    public async Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentation(
            user.Email,
            user.Email,
            user.FirstName,
            user.LastName,
            false,
            true,
            [new CredentialRepresentation(PasswordCredentialType, user.Password, false)]);

        try
        {
            string identityId = await keyCloakClient.RegisterUserAsync(userRepresentation, cancellationToken);

            return identityId;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogError(exception, "User registration failed");

            return Result.Failure<string>(IdentityProviderErrors.EmailTaken);
        }
    }
}
