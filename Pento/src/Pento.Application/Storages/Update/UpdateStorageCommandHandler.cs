using Marten;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.Storages.Update;

internal sealed class UpdateStorageCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> repository,
    IDocumentSession session,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateStorageCommand>
{
    public async Task<Result> Handle(UpdateStorageCommand command, CancellationToken cancellationToken)
    {
        Guid? userHouseholdId = userContext.HouseholdId;
        Storage? storage =  await repository.GetByIdAsync(command.StorageId, cancellationToken);
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
        if (storage.Name != command.Name)
        {
            IReadOnlyList<Guid> foodItemIds = await session.Query<FoodItemDetail>()
                .Where(fi => fi.StorageName == storage.Name)
                .Select(fi => fi.Id).ToListAsync(cancellationToken);
            foreach (Guid foodItemId in foodItemIds)
            {
                session.Events.Append(foodItemId, new FoodItemStorageRenamed(command.Name));
            }
            await session.SaveChangesAsync(cancellationToken);
        }
        if (storage.Notes != command.Notes)
        {
            storage.UpdateNotes(command.Notes);
        }
        repository.Update(storage);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
