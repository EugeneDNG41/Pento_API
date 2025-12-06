using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.FoodReferences.Get;

internal sealed class GetAllFoodReferencesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllFoodReferencesQuery, PagedList<FoodReferenceResponse>>
{
    public async Task<Result<PagedList<FoodReferenceResponse>>> Handle(
        GetAllFoodReferencesQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>();
        var parameters = new DynamicParameters();

        if (request.FoodGroup.HasValue)
        {
            filters.Add("food_group = @FoodGroup");
            parameters.Add("FoodGroup", request.FoodGroup.ToString());
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            filters.Add("(levenshtein(name, @GetAll) <= 2 OR name ILIKE '%' || @GetAll || '%')");
            parameters.Add("GetAll", request.Search);
        }

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string sql = $@"
            SELECT COUNT(*)
            FROM food_references
            {whereClause};

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
            OFFSET @Offset LIMIT @PageSize;
        ";

        parameters.Add("Offset", (request.Page - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);


        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();

        var items = (await multi.ReadAsync<FoodReferenceResponse>())
            .ToList();


        var paged = PagedList<FoodReferenceResponse>.Create(
            items,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result.Success(paged);
    }
}
