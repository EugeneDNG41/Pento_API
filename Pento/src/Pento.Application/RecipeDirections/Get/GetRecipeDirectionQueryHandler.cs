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
using Pento.Domain.RecipeDirections;

namespace Pento.Application.RecipeDirections.Get;

internal sealed class GetRecipeDirectionQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<GetRecipeDirectionQuery, RecipeDirectionResponse>
{
    public async Task<Result<RecipeDirectionResponse>> Handle(
        GetRecipeDirectionQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT
                id AS Id,
                recipe_id AS RecipeId,
                step_number AS StepNumber,
                description AS Description,
                image_url AS ImageUrl,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM recipe_directions
            WHERE id = @RecipeDirectionId
        """;

        RecipeDirectionResponse? direction = await connection.QuerySingleOrDefaultAsync<RecipeDirectionResponse>(
            sql,
            new { request.RecipeDirectionId }
        );

        if (direction is null)
        {
            return Result.Failure<RecipeDirectionResponse>(
                RecipeDirectionErrors.NotFound(request.RecipeDirectionId)
            );
        }

        return Result.Success(direction);
    }
}
