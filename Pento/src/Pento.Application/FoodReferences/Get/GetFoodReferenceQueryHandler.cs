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

        const string sql =
            $"""
            SELECT
                id AS {nameof(FoodReferenceResponse.Id)},
                name AS {nameof(FoodReferenceResponse.Name)},
                food_group AS {nameof(FoodReferenceResponse.FoodGroup)},
                barcode AS {nameof(FoodReferenceResponse.Barcode)},
                brand AS {nameof(FoodReferenceResponse.Brand)},
                typical_shelf_life_days AS {nameof(FoodReferenceResponse.TypicalShelfLifeDays)},
                openfoodfacts_id AS {nameof(FoodReferenceResponse.OpenFoodFactsId)},
                usda_id AS {nameof(FoodReferenceResponse.UsdaId)},
                created_at AS {nameof(FoodReferenceResponse.CreatedAt)},
                updated_at AS {nameof(FoodReferenceResponse.UpdatedAt)}
            FROM food_reference
            WHERE id = @FoodReferenceId
            """;

        FoodReferenceResponse? foodReference = await connection.QuerySingleOrDefaultAsync<FoodReferenceResponse>(sql, request);

        if (foodReference is null)
        {
            return Result.Failure<FoodReferenceResponse>(FoodReferenceErrors.NotFound(request.FoodReferenceId));
        }

        return foodReference;
    }
}

