using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.UploadAvatar;

internal sealed class UploadAvatarCommandHandler(
    IUserContext userContext,
    IBlobService blobService,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UploadAvatarCommand, Uri>
{
    public async Task<Result<Uri>> Handle(UploadAvatarCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<Uri>(UserErrors.NotFound);
        }
        Result<Uri> uploadResult = await blobService.UploadImageAsync(command.File, nameof(User), cancellationToken);
        if (uploadResult.IsFailure)
        {
            return Result.Failure<Uri>(uploadResult.Error);
        }
        user.SetAvatarUrl(uploadResult.Value);
        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.AvatarUrl;
    }
}
