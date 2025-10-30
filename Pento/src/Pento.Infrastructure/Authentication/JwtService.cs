using System.Net.Http;
using System.Net.Http.Json;
using FluentEmail.Core;
using Microsoft.Extensions.Options;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Identity;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Identity;

namespace Pento.Infrastructure.Authentication;

internal sealed class JwtService(HttpClient httpClient, IOptions<KeycloakOptions> options) : IJwtService
{
    private async Task<Result<AuthToken>> RequestTokenAsync(
    IEnumerable<KeyValuePair<string, string>> grantParams,
    string endpoint,
    CancellationToken cancellationToken)
    {
        try
        {
            var parameters = new List<KeyValuePair<string, string>>
        {
            new("client_id", options.Value.ClientId),
            new("client_secret", options.Value.ClientSecret),
        };
            parameters.AddRange(grantParams);

            using var content = new FormUrlEncodedContent(parameters);

            using HttpResponseMessage response = await httpClient.PostAsync(
                endpoint,
                content,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            AuthToken? token = await response.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);
            return token is null
                ? Result.Failure<AuthToken>(IdentityProviderErrors.AuthenticationFailed)
                : token;
        }
        catch (HttpRequestException)
        {
            return Result.Failure<AuthToken>(IdentityProviderErrors.AuthenticationFailed);
        }
    }
    public Task<Result<AuthToken>> RevokeAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        KeyValuePair<string, string>[] grantParams = new[]
        {
        new KeyValuePair<string,string>("token", accessToken),
        new KeyValuePair<string,string>("token_type_hint", "access_token")
        };
        return RequestTokenAsync(grantParams, "revoke", cancellationToken);
    }
    public Task<Result<AuthToken>> RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        KeyValuePair<string, string>[] grantParams = new[]
        {
        new KeyValuePair<string,string>("token", refreshToken),
        new KeyValuePair<string,string>("token_type_hint", "refresh_token")
        };
        return RequestTokenAsync(grantParams, "revoke", cancellationToken);
    }
    public Task<Result<AuthToken>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        KeyValuePair<string, string>[] grantParams = new[]
        {
        new KeyValuePair<string,string>("grant_type", "refresh_token"),
        new KeyValuePair<string,string>("refresh_token", refreshToken),
        };
        return RequestTokenAsync(grantParams, "token", cancellationToken);
    }

    public Task<Result<AuthToken>> GetAuthTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        KeyValuePair<string, string>[] grantParams = new[]
        {
        new KeyValuePair<string,string>("scope", "openid email"),
        new KeyValuePair<string,string>("grant_type", "password"),
        new KeyValuePair<string,string>("username", email),
        new KeyValuePair<string,string>("password", password),
        };
        return RequestTokenAsync(grantParams, "token", cancellationToken);
    }
}
