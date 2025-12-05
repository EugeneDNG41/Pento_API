using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Notifications.Create;

internal sealed class CreateNotificationCommandHandler(
    INotificationService notificationService,
    IUserContext userContext
) : ICommandHandler<CreateNotificationCommand>
{
    public async Task<Result> Handle(CreateNotificationCommand command, CancellationToken cancellationToken)
    {
        if(userContext.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(
                UserErrors.NotFound
            );
        }
        Result result = await notificationService.SendToUserAsync(userContext.UserId, command.Title, command.Body, command.Type, command.Payload, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(
                result.Error
            );
        }
        return Result.Success();
    }
}

