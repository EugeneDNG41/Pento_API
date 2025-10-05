using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Identity;

public interface IIdentityProviderService
{
    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<Result<AuthToken>> GetAuthTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
