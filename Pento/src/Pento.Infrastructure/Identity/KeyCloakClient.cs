using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Pento.Application.Abstractions.Identity;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Authentication;

namespace Pento.Infrastructure.Identity;

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
    internal async Task SendVerificationEmailAsync(string identityId, CancellationToken cancellationToken = default)
    {
        string[] actions = new[] { "VERIFY_EMAIL" };
        HttpResponseMessage httpResponseMessage = await httpClient.PutAsJsonAsync(
            $"users/{identityId}/execute-actions-email",
            actions,
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
