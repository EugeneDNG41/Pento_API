using System.Data.Common;
using Dapper;
using Marten;
using Marten.Pagination;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Projections;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Compartments.GetAll;
internal sealed class GetCompartmentsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory,
    IQuerySession querySession) 
    : IQueryHandler<GetCompartmentsQuery, IReadOnlyList<CompartmentWithFoodItemPreviewResponse>>
{
    public async Task<Result<IReadOnlyList<CompartmentWithFoodItemPreviewResponse>>> Handle(
        GetCompartmentsQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<IReadOnlyList<CompartmentWithFoodItemPreviewResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
            SELECT
                id AS {nameof(CompartmentResponse.Id)},
                storage_id AS {nameof(CompartmentResponse.StorageId)},
                household_id AS {nameof(CompartmentResponse.HouseholdId)},
                name AS {nameof(CompartmentResponse.Name)},
                notes AS {nameof(CompartmentResponse.Notes)}
            FROM compartments
            WHERE storage_id = @StorageId
            """;
        CommandDefinition command = new(sql, query, cancellationToken: cancellationToken);
        List<CompartmentResponse> compartments = (await connection.QueryAsync<CompartmentResponse>(command)).AsList();
        var result = new List<CompartmentWithFoodItemPreviewResponse>();

        foreach (CompartmentResponse c in compartments)
        {
            IPagedList<FoodItemPreview> previews =
                await querySession.Query<FoodItemPreview>()
                    .Where(p => p.CompartmentId == c.Id)
                    .ToPagedListAsync(1, 5, cancellationToken);

            result.Add(new CompartmentWithFoodItemPreviewResponse(
                c.Id, c.StorageId, c.HouseholdId, c.Name, c.Notes, previews));
        }
        return result;
    }
}
