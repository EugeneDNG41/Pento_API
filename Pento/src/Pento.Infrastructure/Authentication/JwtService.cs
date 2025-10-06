using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Identity;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Identity;

namespace Pento.Infrastructure.Authentication;

internal sealed class JwtService(HttpClient httpClient, IOptions<KeyCloakOptions> options) : IJwtService
{
    public async Task<Result<AuthToken>> GetAuthTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", options.Value.ConfidentialClientId),
                new("client_secret", options.Value.ConfidentialClientSecret),
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
}
