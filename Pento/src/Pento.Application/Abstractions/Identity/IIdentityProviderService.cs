using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Identity;

public interface IIdentityProviderService
{
    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<Result> SendVerificationEmailAsync(string identityId, CancellationToken cancellationToken = default);
}
