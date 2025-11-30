using Pento.Application.Abstractions.ThirdPartyServices.Identity;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Authentication;

public interface IJwtService
{
    Task<Result<AuthToken>> GetAuthTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
    Task<Result<AuthToken>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<AuthToken>> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<AuthToken>> RevokeAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
