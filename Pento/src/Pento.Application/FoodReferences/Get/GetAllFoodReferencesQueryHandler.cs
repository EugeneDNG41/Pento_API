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

namespace Pento.Application.FoodReferences.Get;
internal sealed class GetAllFoodReferencesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllFoodReferencesQuery, IReadOnlyList<FoodReferenceResponse>>
{
    public async Task<Result<IReadOnlyList<FoodReferenceResponse>>> Handle(
        GetAllFoodReferencesQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        string sql = """
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
            """;

        if (request.FoodGroup.HasValue)
        {
            sql += " WHERE food_group = @FoodGroup";
        }

        sql += " ORDER BY name;";

        IEnumerable<FoodReferenceResponse> foodReferences = await connection.QueryAsync<FoodReferenceResponse>(
            sql,
            new { FoodGroup = request.FoodGroup?.ToString() } 
        );

        return Result.Success<IReadOnlyList<FoodReferenceResponse>>(foodReferences.ToList());
    }
}
