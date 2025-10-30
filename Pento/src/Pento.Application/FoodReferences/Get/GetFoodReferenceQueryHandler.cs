using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Get;
internal sealed class GetFoodReferenceQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetFoodReferenceQuery, FoodReferenceResponse>
{
    public async Task<Result<FoodReferenceResponse>> Handle(GetFoodReferenceQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                food_group AS FoodGroup,
                data_type AS DataType,                         
                notes AS Notes,                                 
                typical_shelf_life_days_pantry AS TypicalShelfLifeDays_Pantry,
                typical_shelf_life_days_fridge AS TypicalShelfLifeDays_Fridge,
                typical_shelf_life_days_freezer AS TypicalShelfLifeDays_Freezer,
                image_url AS ImageUrl,
                brand AS Brand,
                barcode AS Barcode,
                created_on_utc AS CreatedAt,
                updated_on_utc AS UpdatedAt
            FROM food_references
            WHERE id = @FoodReferenceId
            """;
        FoodReferenceResponse? foodReference = await connection.QueryFirstOrDefaultAsync<FoodReferenceResponse>(
            sql, new { request.FoodReferenceId });

        if (foodReference is null)
        {
            return Result.Failure<FoodReferenceResponse>(
                FoodReferenceErrors.NotFound(request.FoodReferenceId));
        }

        return Result.Success(foodReference);
    }
}

