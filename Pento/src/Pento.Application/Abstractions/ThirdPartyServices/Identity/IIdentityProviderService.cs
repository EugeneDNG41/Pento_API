using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.ThirdPartyServices.Identity;

public interface IIdentityProviderService
{
    Task<Result> ChangePasswordAsync(string identityId, string newPassword, CancellationToken cancellationToken = default);
    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<Result> SendResetPasswordEmailAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result> SendVerificationEmailAsync(string identityId, CancellationToken cancellationToken = default);
}
