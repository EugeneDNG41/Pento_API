
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.RefreshToken;

internal sealed class RefreshTokenCommandHandler(IJwtService jwtService) : ICommandHandler<RefreshTokenCommand, AuthToken>
{
    public async Task<Result<AuthToken>> Handle(
        RefreshTokenCommand command, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure<AuthToken>(UserErrors.InvalidToken);
        }
        Result<AuthToken> result = await jwtService.RefreshTokenAsync(command.RefreshToken, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<AuthToken>(UserErrors.InvalidToken);
        }
        return result;
    }
}
