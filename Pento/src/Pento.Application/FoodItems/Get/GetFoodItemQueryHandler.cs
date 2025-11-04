using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;


namespace Pento.Application.FoodItems.Get;

internal sealed class GetFoodItemQueryHandler(
    IQuerySession querySession)
    : IQueryHandler<GetFoodItemQuery, FoodItemDetail>
{
    public async Task<Result<FoodItemDetail>> Handle(GetFoodItemQuery request, CancellationToken cancellationToken)
    {

        FoodItemDetail? foodItem = await querySession.LoadAsync<FoodItemDetail>(request.Id, cancellationToken);
        if ( foodItem is null)
        {
           return Result.Failure<FoodItemDetail>(FoodItemErrors.NotFound);
        }
        return foodItem;
    }
}
