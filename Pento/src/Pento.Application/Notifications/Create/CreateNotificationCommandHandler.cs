using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Application.MealPlans.Create.FromRecipe;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;
using Pento.Domain.Notifications;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.Notifications.Create;

internal sealed class CreateNotificationCommandHandler(
    IGenericRepository<Notification> notificationRepo,
    IGenericRepository<DeviceToken> deviceTokenRepo,
    INotificationSender notificationSender,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateNotificationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        if(userContext.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(
                UserErrors.NotFound
            );
        }
        var noti = Notification.Create(
            userContext.UserId,
            request.Title,
            request.Body,
            request.Type,
            request.DataJson
        );

        notificationRepo.Add(noti);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        IEnumerable<DeviceToken> tokens = await deviceTokenRepo.FindAsync(
            x => x.UserId == userContext.UserId,
            cancellationToken
        );
        if (!tokens.Any())
        {
            return Result.Failure<Guid>(
            DeviceTokenErrors.NotFound
      );
        }

        foreach (DeviceToken token in tokens)
        {
            await notificationSender.SendAsync(
                token.Token,
                noti.Title,
                noti.Body,
                cancellationToken
            );
        }

        noti.MarkSent();
        notificationRepo.Update(noti);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(noti.Id);
    }
}

