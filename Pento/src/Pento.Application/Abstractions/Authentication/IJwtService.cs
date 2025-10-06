using Pento.Application.Abstractions.Identity;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Authentication;

public interface IJwtService
{
    Task<Result<AuthToken>> GetAuthTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
