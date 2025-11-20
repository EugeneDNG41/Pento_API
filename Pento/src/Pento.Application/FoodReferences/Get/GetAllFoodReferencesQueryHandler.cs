using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.FoodReferences.Get;

internal sealed class GetAllFoodReferencesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllFoodReferencesQuery, PagedFoodReferencesResponse>
{
    public async Task<Result<PagedFoodReferencesResponse>> Handle(
        GetAllFoodReferencesQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        var filters = new List<string>();
        var parameters = new DynamicParameters();

        if (request.FoodGroup.HasValue)
        {
            filters.Add("food_group = @FoodGroup");
            parameters.Add("FoodGroup", request.FoodGroup.ToString());
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            filters.Add("levenshtein(name, @Search) <= 2");
            parameters.Add("Search", $"%{request.Search}%");
        }

        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;

        string sqlCount = $"""
            SELECT COUNT(*) 
            FROM food_references
            {whereClause};
        """;

        string sqlData = $"""
            SELECT
                id AS Id,
                name AS Name,
                food_group AS FoodGroup,
                typical_shelf_life_days_pantry AS TypicalShelfLifeDays_Pantry,
                typical_shelf_life_days_fridge AS TypicalShelfLifeDays_Fridge,
                typical_shelf_life_days_freezer AS TypicalShelfLifeDays_Freezer,
                added_by AS AddedBy,
                image_url AS ImageUrl,
                brand AS Brand,
                barcode AS Barcode,
                unit_type AS UnitType,
                created_on_utc AS CreatedAt,
                updated_on_utc AS UpdatedAt
            FROM food_references
            {whereClause}
            ORDER BY name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (request.Page - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        int totalCount = await connection.ExecuteScalarAsync<int>(sqlCount, parameters);
        IEnumerable<FoodReferenceResponse> items = await connection.QueryAsync<FoodReferenceResponse>(sqlData, parameters);

        var response = new PagedFoodReferencesResponse
        {
            Items = items.ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result.Success(response);
    }
}
