﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients;

namespace Pento.Application.RecipeIngredients.Get;
internal sealed class GetRecipeIngredientQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetRecipeIngredientQuery, RecipeIngredientResponse>
{
    public async Task<Result<RecipeIngredientResponse>> Handle(
        GetRecipeIngredientQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
            SELECT
                id AS {nameof(RecipeIngredientResponse.Id)},
                recipe_id AS {nameof(RecipeIngredientResponse.RecipeId)},
                foodref_id AS {nameof(RecipeIngredientResponse.FoodRefId)},
                quantity AS {nameof(RecipeIngredientResponse.Quantity)},
                unit_id AS {nameof(RecipeIngredientResponse.UnitId)},
                notes AS {nameof(RecipeIngredientResponse.Notes)},
                created_on_utc AS {nameof(RecipeIngredientResponse.CreatedOnUtc)},
                updated_on_utc AS {nameof(RecipeIngredientResponse.UpdatedOnUtc)}
            FROM recipe_ingredients
            WHERE id = @RecipeIngredientId
            """;

        RecipeIngredientResponse? ingredient = await connection.QuerySingleOrDefaultAsync<RecipeIngredientResponse>(
            sql,
            new { recipeIngredientId = request.RecipeIngredientId }
        );


        if (ingredient is null)
        {
            return Result.Failure<RecipeIngredientResponse>(RecipeIngredientErrors.NotFound(request.RecipeIngredientId));
        }

        return ingredient;
    }
}
