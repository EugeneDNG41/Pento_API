using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.StorageItems.Create;

internal sealed class CreateStorageItemCommandHandler(/*IFoodItemRepository repository, IUnitOfWork unitOfWork*/)
    : ICommandHandler<CreateStorageItemCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateStorageItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
