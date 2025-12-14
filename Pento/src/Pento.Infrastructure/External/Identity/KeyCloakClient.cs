using System.Net.Http.Json;

namespace Pento.Infrastructure.External.Identity;

internal sealed class KeyCloakClient(HttpClient httpClient)
{
    internal async Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(
            "users",
            user,
            cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return ExtractIdentityIdFromLocationHeader(httpResponseMessage);
    }
    internal async Task ChangePasswordAsync(
        string identityId,
        CredentialRepresentation credentialRepresentation,
        CancellationToken cancellationToken = default)
    {
        await httpClient.PutAsJsonAsync(
            $"users/{identityId}/reset-password",
            credentialRepresentation,
            cancellationToken);
    }
    internal async Task SendResetPasswordEmailAsync(string identityId, CancellationToken cancellationToken = default)
    {
        string[] actions = new[] { "UPDATE_PASSWORD" };
        await httpClient.PutAsJsonAsync(
            $"users/{identityId}/execute-actions-email",
            actions,
            cancellationToken);

    }
    internal async Task SendVerificationEmailAsync(string identityId, CancellationToken cancellationToken = default)
    {
        string[] actions = new[] { "VERIFY_EMAIL" };
        await httpClient.PutAsJsonAsync(
            $"users/{identityId}/execute-actions-email",
            actions,
            cancellationToken);
    }
    internal async Task SignOutAllSessionsAsync(string identityId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(
            $"users/{identityId}/logout",
            cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();
    }

    private static string ExtractIdentityIdFromLocationHeader(
        HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";

        string? locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header is null");
        }

        int userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        string identityId = locationHeader.Substring(userSegmentValueIndex + usersSegmentName.Length);

        return identityId;
    }
}
