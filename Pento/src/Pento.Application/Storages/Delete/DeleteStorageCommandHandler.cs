using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Delete;

internal sealed class DeleteStorageCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<FoodItem> foodItemRepository,
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
        bool hasFoodItems = await foodItemRepository.AnyAsync(fi => compartments.Select(c => c.Id).Contains(fi.CompartmentId), cancellationToken);
        if (hasFoodItems)
        {
            return Result.Failure(StorageErrors.NotEmpty);
        }
        bool otherStorageExists = !await storageRepository.AnyAsync(s => s.HouseholdId == currentHouseholdId && s.Id != storage.Id, cancellationToken);
        if (otherStorageExists)
        {
            return Result.Failure(StorageErrors.AtLeastOne);
        }
        storage.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
