using Newtonsoft.Json.Linq;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
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


        IEnumerable<DeviceToken>? tokens = await deviceTokenRepository.FindAsync(
           x => x.UserId == userId,
           cancellationToken
       );
        DeviceToken? existing = tokens.FirstOrDefault();

        if (existing is null)
        {
            var token = new DeviceToken(userId, request.Token, request.Platform);
            deviceTokenRepository.Add(token);
        }
        else
        {
            existing.UpdateToken(request.Token);
            deviceTokenRepository.Update(existing);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.Token);
    }
}
