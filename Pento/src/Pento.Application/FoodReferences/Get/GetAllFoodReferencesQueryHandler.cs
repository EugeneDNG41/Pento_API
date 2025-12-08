using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.FoodReferences.Get;

internal sealed class GetAllFoodReferencesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllFoodReferencesQuery, PagedList<FoodReferenceResponse>>
{
    public async Task<Result<PagedList<FoodReferenceResponse>>> Handle(
        GetAllFoodReferencesQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetAllFoodReferencesSortBy.Name => "name",
            GetAllFoodReferencesSortBy.FoodGroup => "food_group",
            GetAllFoodReferencesSortBy.Brand => "brand",
            GetAllFoodReferencesSortBy.CreatedAt => "created_on_utc",
            _ => "id"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder}";
        var filters = new List<string>();
        var parameters = new DynamicParameters();

        if (query.FoodGroup != null && query.FoodGroup.Length > 0)
        {
            filters.Add("food_group = Any(@FoodGroup::text[])");
            parameters.Add("FoodGroup", query.FoodGroup.Select(x => x.ToString()).ToArray());
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            filters.Add("(levenshtein(name, @GetAll) <= 2 OR name ILIKE '%' || @GetAll || '%')");
            parameters.Add("GetAll", query.Search);
        }
        if (query.HasImage.HasValue)
        {
            if (query.HasImage.Value)
            {
                filters.Add("image_url IS NOT NULL");
            }
            else
            {
                filters.Add("image_url IS NULL");
            }
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
            {orderClause}
            OFFSET @Offset LIMIT @PageSize;
        ";

        parameters.Add("Offset", (query.Page - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);


        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();

        var items = (await multi.ReadAsync<FoodReferenceResponse>())
            .ToList();


        var paged = PagedList<FoodReferenceResponse>.Create(
            items,
            totalCount,
            query.Page,
            query.PageSize
        );

        return Result.Success(paged);
    }
}
