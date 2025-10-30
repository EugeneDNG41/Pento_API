using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.StorageItems;


namespace Pento.Application.StorageItems.Get;

internal sealed class GetStorageItemQueryHandler(IStorageItemRepository repository)
    : IQueryHandler<GetStorageItemQuery, StorageItemResponse>
{
    public async Task<Result<StorageItemResponse>> Handle(GetStorageItemQuery request, CancellationToken cancellationToken)
    {
        StorageItem? storageItem = await repository.GetByIdAsync(request.Id, cancellationToken);
        if ( storageItem is null)
        {
           return Result.Failure<StorageItemResponse>(StorageItemErrors.NotFound(request.Id));
        }
        var response = new StorageItemResponse
        (
            storageItem.Id,
            storageItem.FoodRefId,
            storageItem.CompartmentId,
            storageItem.CustomName,
            storageItem.Quantity,
            storageItem.UnitId,
            storageItem.ExpirationDateUtc,
            storageItem.Notes
       );
        return response;
    }
}
