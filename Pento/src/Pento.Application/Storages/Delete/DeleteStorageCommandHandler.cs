using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Delete;

internal sealed class DeleteStorageCommandHandler(
    IUserContext userContext,
    ICompartmentService compartmentService,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteStorageCommand>
{
    public async Task<Result> Handle(DeleteStorageCommand request, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        Storage? storage = await storageRepository.GetByIdAsync(request.StorageId, cancellationToken);
        if (storage is null)
        {
            return Result.Failure(StorageErrors.NotFound);
        }
        if (storage.HouseholdId != currentHouseholdId)
        {
            return Result.Failure(StorageErrors.ForbiddenAccess);
        }
        IEnumerable<Compartment> compartments = await compartmentRepository.FindAsync(c => c.StorageId == storage.Id, cancellationToken);
        foreach (Compartment compartment in compartments)
        {
            Result checkEmptyResult = await compartmentService.CheckIfEmptyAsync(compartment.Id, currentHouseholdId.Value, cancellationToken);
            if (checkEmptyResult.IsFailure)
            {
                return checkEmptyResult;
            }
        }
        bool otherStorageExists = !await storageRepository.AnyAsync(s => s.HouseholdId == currentHouseholdId && s.Id != storage.Id, cancellationToken);
        if (otherStorageExists)
        {
            return Result.Failure(StorageErrors.AtLeastOne);
        }
        await storageRepository.RemoveAsync(storage, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
