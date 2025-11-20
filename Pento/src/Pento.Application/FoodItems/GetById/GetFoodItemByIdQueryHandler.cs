
using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Users;


namespace Pento.Application.FoodItems.GetById;
internal sealed class GetFoodItemByIdQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetFoodItemByIdQuery, FoodItemDetail>
{
    public async Task<Result<FoodItemDetail>> Handle(GetFoodItemByIdQuery query, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId == Guid.Empty)
        {
            return Result.Failure<FoodItemDetail>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        var filters = new List<string>
        {
            "fi.is_deleted IS FALSE",
            "fi.is_archived IS FALSE",
            "fi.id = @Id",
            "fi.household_id = @HouseholdId"
        };
        var parameters = new DynamicParameters();
        parameters.Add("Id", query.Id);
        parameters.Add("HouseholdId", currentHouseholdId);

        string whereClause = "WHERE " + string.Join(" AND ", filters);

        string sql = $"""
            SELECT
                fi.id                    AS {nameof(FoodItemDetailRow.Id)},
                fr.name                  AS {nameof(FoodItemDetailRow.FoodReferenceName)},
                s.name                   AS {nameof(FoodItemDetailRow.StorageName)},
                s.type                   AS {nameof(FoodItemDetailRow.StorageType)},              
                c.name                   AS {nameof(FoodItemDetailRow.CompartmentName)},
                fr.brand                 AS {nameof(FoodItemDetailRow.Brand)},
                fr.food_group            AS {nameof(FoodItemDetailRow.FoodGroup)},
                fi.name                  AS {nameof(FoodItemDetailRow.Name)},
                fi.image_url             AS {nameof(FoodItemDetailRow.ImageUrl)},
                fi.quantity              AS {nameof(FoodItemDetailRow.Quantity)},
                u.abbreviation           AS {nameof(FoodItemDetailRow.UnitAbbreviation)},
                u.type                   AS {nameof(FoodItemDetailRow.UnitType)},
                fi.expiration_date       AS {nameof(FoodItemDetailRow.ExpirationDate)},
                fr.typical_shelf_life_days_pantry   AS {nameof(FoodItemDetailRow.TypicalPantryShelfLifeDays)},
                fr.typical_shelf_life_days_fridge   AS {nameof(FoodItemDetailRow.TypicalFridgeShelfLifeDays)},
                fr.typical_shelf_life_days_freezer  AS {nameof(FoodItemDetailRow.TypicalFreezerShelfLifeDays)},
                fi.notes                 AS {nameof(FoodItemDetailRow.Notes)},
                fi.added_by              AS {nameof(FoodItemDetailRow.AddedById)},
                fi.last_modified_by      AS {nameof(FoodItemDetailRow.LastModifiedById)}
            FROM food_items fi
            LEFT JOIN food_references fr ON fr.id = fi.food_reference_id
            LEFT JOIN compartments c    ON c.id  = fi.compartment_id
            LEFT JOIN storages s        ON s.id  = c.storage_id            
            LEFT JOIN units u           ON u.id  = fi.unit_id
            {whereClause}
            LIMIT 1;
            """;

        FoodItemDetailRow? row = await connection.QuerySingleOrDefaultAsync<FoodItemDetailRow>(sql, parameters);
        if (row is null)
        {
            return Result.Failure<FoodItemDetail>(FoodItemErrors.NotFound);
        }

        // Fetch users (handles null last_modified_by)
        var userIds = new List<Guid> { row.AddedById };
        if (row.LastModifiedById is Guid lmId)
        {
            userIds.Add(lmId);
        }

        const string userSql = """
            SELECT 
                id AS UserId, first_name AS FirstName, last_name AS LastName, avatar_url AS AvatarUrl
            FROM users
            WHERE id = @AddedById;

            SELECT 
                id AS UserId, first_name AS FirstName, last_name AS LastName, avatar_url AS AvatarUrl
            FROM users
            WHERE id = @LastModifiedById;
            """;

        // If LastModifiedById is null, the second SELECT returns 0 rows (fine).
        SqlMapper.GridReader grid = await connection.QueryMultipleAsync(
            userSql,
            new
            {
                row.AddedById,
                row.LastModifiedById
            }
        );

        BasicUserResponse? addedBy = await grid.ReadSingleOrDefaultAsync<BasicUserResponse>();
        BasicUserResponse? lastModifiedBy = await grid.ReadSingleOrDefaultAsync<BasicUserResponse>();
        if (addedBy is null)
        {
            return Result.Failure<FoodItemDetail>(UserErrors.NotFound);
        }
        var detail = new FoodItemDetail(
            row.Id,
            row.FoodReferenceName,
            row.StorageName,
            row.StorageType.ToString(),
            row.CompartmentName,
            row.Name,
            row.Brand,
            row.FoodGroup.ToReadableString(),
            row.ImageUrl,
            row.Quantity,
            row.UnitAbbreviation,
            row.UnitType,
            row.ExpirationDate,
            row.TypicalPantryShelfLifeDays,
            row.TypicalFridgeShelfLifeDays,
            row.TypicalFreezerShelfLifeDays,
            row.Notes,
            addedBy,
            lastModifiedBy
        );
        return detail;
    }
}
