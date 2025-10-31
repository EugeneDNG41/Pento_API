using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.Storages.Update;

internal sealed class UpdateStorageCommandHandler(
    IGenericRepository<Storage> repository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateStorageCommand>
{
    public async Task<Result> Handle(UpdateStorageCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        Storage? storage =  await repository.GetByIdAsync(command.StorageId, cancellationToken);
        if (storage is null)
        {
            return Result.Failure(StorageErrors.NotFound);
        }
        if (storage.HouseholdId != command.HouseholdId.Value)
        {
            return Result.Failure(StorageErrors.ForbiddenAccess);
        }
        storage.Update(command.Name, command.Type, command.Notes);
        repository.Update(storage);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
