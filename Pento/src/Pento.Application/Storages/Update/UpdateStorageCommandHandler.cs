using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Update;

internal sealed class UpdateStorageCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> repository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateStorageCommand>
{
    public async Task<Result> Handle(UpdateStorageCommand command, CancellationToken cancellationToken)
    {
        Guid? userHouseholdId = userContext.HouseholdId;
        Storage? storage = await repository.GetByIdAsync(command.StorageId, cancellationToken);
        if (storage is null)
        {
            return Result.Failure(StorageErrors.NotFound);
        }
        if (storage.HouseholdId != userHouseholdId)
        {
            return Result.Failure(StorageErrors.ForbiddenAccess);
        }
        if (storage.Name == command.Name && storage.Notes == command.Notes)
        {
            return Result.Success();
        }
        if (await repository.AnyAsync(s => s.Name == command.Name && s.Id != storage.Id && s.HouseholdId == userHouseholdId, cancellationToken))
        {
            return Result.Failure(StorageErrors.DuplicateName);
        }
        storage.UpdateName(command.Name);
        storage.UpdateNotes(command.Notes);
        repository.Update(storage);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
