using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;


namespace Pento.Application.StorageItems.Get;

internal sealed class GetStorageItemQueryHandler(/*ISqlConnectionFactory dbConnectionFactory*/)
    : IQueryHandler<GetStorageItemQuery, StorageItemResponse>
{
    public Task<Result<StorageItemResponse>> Handle(GetStorageItemQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
