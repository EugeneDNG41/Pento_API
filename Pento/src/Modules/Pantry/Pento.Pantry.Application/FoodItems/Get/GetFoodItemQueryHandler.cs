using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Caching;
using Pento.Common.Application.Data;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Pantry.Application.FoodItems.Get;

internal sealed class GetFoodItemQueryHandler(/*IDbConnectionFactory dbConnectionFactory*/)
    : IQueryHandler<GetFoodItemQuery, FoodItemResponse>
{
    public Task<Result<FoodItemResponse>> Handle(GetFoodItemQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
