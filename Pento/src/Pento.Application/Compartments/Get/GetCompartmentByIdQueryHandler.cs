using System.ComponentModel;
using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.FoodItems.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Units;

namespace Pento.Application.Compartments.Get;

internal sealed class GetCompartmentByIdQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory connectionFactory) : IQueryHandler<GetCompartmentByIdQuery, CompartmentWithFoodItemPreviewResponse>
{
    public async Task<Result<CompartmentWithFoodItemPreviewResponse>> Handle(GetCompartmentByIdQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<CompartmentWithFoodItemPreviewResponse>(HouseholdErrors.NotInAnyHouseHold);
        }
        await using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        var filters = new List<string>
        {
            "fi.is_deleted IS FALSE",
            "fi.compartment_id = @CompartmentId"

        };
        var parameters = new DynamicParameters();
        parameters.Add("CompartmentId", query.CompartmentId);

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            filters.Add("(LOWER(name) LIKE LOWER(@SearchText))");
            parameters.Add("SearchText", $"%{query.SearchText}%");
        }

        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;

        string sql = $"""
            SELECT
                id AS {nameof(CompartmentResponse.Id)},
                storage_id AS {nameof(CompartmentResponse.StorageId)},
                household_id AS {nameof(CompartmentResponse.HouseholdId)},
                name AS {nameof(CompartmentResponse.Name)},
                notes AS {nameof(CompartmentResponse.Notes)}
            FROM compartments
            WHERE id = @CompartmentId AND is_deleted = false;

            SELECT COUNT(*) 
                FROM food_items fi
                {whereClause};
            SELECT
                fi.id AS {nameof(FoodItemPreviewRow.Id)},
                fi.name AS {nameof(FoodItemPreviewRow.Name)},
                fr.food_group AS {nameof(FoodItemPreviewRow.FoodGroup)},
                fi.image_url AS {nameof(FoodItemPreviewRow.ImageUrl)},
                fi.quantity AS {nameof(FoodItemPreviewRow.Quantity)},
                u.abbreviation AS {nameof(FoodItemPreviewRow.UnitAbbreviation)},
                expiration_date AS {nameof(FoodItemPreviewRow.ExpirationDate)}
            FROM food_items fi
            LEFT JOIN food_references fr ON fi.food_reference_id = fr.id
            LEFT JOIN units u ON fi.unit_id = u.id
            {whereClause}
            ORDER BY name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);
        CompartmentResponse? compartment =  await multi.ReadFirstOrDefaultAsync<CompartmentResponse>();
        if (compartment is null)
        {
            return Result.Failure<CompartmentWithFoodItemPreviewResponse>(CompartmentErrors.NotFound);
        }
        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<FoodItemPreviewRow> rows = await multi.ReadAsync<FoodItemPreviewRow>();

        var pagedResponse = PagedList<FoodItemPreview>.Create(
            rows.Select(r => new FoodItemPreview(
                                r.Id,
                                r.Name,
                                r.FoodGroup.ToReadableString(), // <-- apply your extension here
                                r.ImageUrl,
                                r.Quantity,
                                r.UnitAbbreviation,
                                r.ExpirationDate
                            )).ToList(),
            totalCount,
            query.PageNumber,
            query.PageSize);


        var response = new CompartmentWithFoodItemPreviewResponse(
            compartment.Id, compartment.StorageId, compartment.HouseholdId, compartment.Name, compartment.Notes, pagedResponse);
        
        return response;
    }
}
