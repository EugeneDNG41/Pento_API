using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;


namespace Pento.Application.FoodItems.Get;

internal sealed class GetFoodItemQueryHandler(
    IGenericRepository<FoodItem> repository)
    : IQueryHandler<GetFoodItemQuery, FoodItemResponse>
{
    public async Task<Result<FoodItemResponse>> Handle(GetFoodItemQuery request, CancellationToken cancellationToken)
    {
        FoodItem? foodItem = await repository.GetByIdAsync(request.Id, cancellationToken);
        if ( foodItem is null)
        {
           return Result.Failure<FoodItemResponse>(FoodItemErrors.NotFound(request.Id));
        }
        var response = new FoodItemResponse
        (
            foodItem.Id,
            foodItem.FoodRefId,
            foodItem.CompartmentId,
            foodItem.CustomName,
            foodItem.Quantity,
            foodItem.UnitId,
            foodItem.ExpirationDateUtc,
            foodItem.Notes
       );
        return response;
    }
}
