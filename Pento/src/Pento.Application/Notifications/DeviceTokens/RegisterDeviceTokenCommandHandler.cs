using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;

namespace Pento.Application.Notifications.DeviceTokens;

internal sealed class RegisterDeviceTokenCommandHandler(
    IUserContext userContext,
    IGenericRepository<DeviceToken> deviceTokenRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<RegisterDeviceTokenCommand, string>
{
    public async Task<Result<string>> Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Result.Failure<string>(DeviceTokenErrors.NotFound);
        }

        Guid userId = userContext.UserId;

        DeviceToken? usedByOtherUser = (await deviceTokenRepository.FindAsync(
            x => x.Token == request.Token && x.UserId != userId,
            cancellationToken
        )).FirstOrDefault();

        if (usedByOtherUser is not null)
        {
            usedByOtherUser.ReassignTo(userId, request.Platform);
            await deviceTokenRepository.UpdateAsync(usedByOtherUser, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(request.Token);
        }

        DeviceToken? duplicate = (await deviceTokenRepository.FindAsync(
            x => x.UserId == userId && x.Token == request.Token,
            cancellationToken
        )).FirstOrDefault();

        if (duplicate is not null)
        {
            return Result.Success(request.Token);
        }

        DeviceToken? existing = (await deviceTokenRepository.FindAsync(
            x => x.UserId == userId,
            cancellationToken
        )).FirstOrDefault();

        if (existing is null)
        {
            var token = new DeviceToken(userId, request.Token, request.Platform);
            deviceTokenRepository.Add(token);
        }
        else
        {
            existing.UpdateToken(request.Token);
            await deviceTokenRepository.UpdateAsync(existing, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.Token);
    }

}
