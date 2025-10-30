using JasperFx.Events.Daemon;
using Marten;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.StorageItems;
using Pento.Domain.StorageItems.Events;

namespace Pento.Application.StorageItems.Create;

internal sealed class CreateStorageItemCommandHandler(IStorageItemRepository repository)
    : ICommandHandler<CreateStorageItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStorageItemCommand command, CancellationToken cancellationToken)
    {
        var e = new StorageItemCreated(
            Guid.CreateVersion7(),
            command.FoodRefId,
            command.CompartmentId,
            command.CustomName,
            command.Quantity,
            command.UnitId,
            command.ExpirationDateUtc,
            command.Notes);
        await repository.StartStreamAsync(e, cancellationToken);
        return e.Id;
    }
}
