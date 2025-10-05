using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Pento.Application.Abstractions.Identity;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Authentication;

namespace Pento.Infrastructure.Identity;

internal sealed class KeyCloakClient(HttpClient httpClient, IOptions<KeyCloakOptions> options)
{
    private readonly KeyCloakOptions _options = options.Value;
    public async Task<Result<AuthToken>> GetAuthTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", _options.ConfidentialClientId),
                new("client_secret", _options.ConfidentialClientSecret),
                new("scope", "openid email"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password)
            };

            using var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

            HttpResponseMessage response = await httpClient.PostAsync(
                "",
                authorizationRequestContent,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            AuthToken? authorizationToken = await response
                .Content
                .ReadFromJsonAsync<AuthToken>(cancellationToken);

            if (authorizationToken is null)
            {
                return Result.Failure<AuthToken>(IdentityProviderErrors.AuthenticationFailed);
            }

            return authorizationToken;
        }
        catch (HttpRequestException)
        {
            return Result.Failure<AuthToken>(IdentityProviderErrors.AuthenticationFailed);
        }
    }
    internal async Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(
            "users",
            user,
            cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return ExtractIdentityIdFromLocationHeader(httpResponseMessage);
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
