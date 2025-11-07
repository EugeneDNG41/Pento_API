using System.ComponentModel;
using System.Data.Common;
using Dapper;
using Marten;
using Marten.Internal.Sessions;
using Marten.Pagination;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems.Projections;
using Pento.Domain.Units;

namespace Pento.Application.Compartments.Get;

internal sealed class GetCompartmentByIdQueryHandler(
    ISqlConnectionFactory connectionFactory,
    IQuerySession session) : IQueryHandler<GetCompartmentByIdQuery, CompartmentWithFoodItemPreviewResponse>
{
    public async Task<Result<CompartmentWithFoodItemPreviewResponse>> Handle(GetCompartmentByIdQuery query, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await connectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
            SELECT
                id AS {nameof(CompartmentResponse.Id)},
                storage_id AS {nameof(CompartmentResponse.StorageId)},
                household_id AS {nameof(CompartmentResponse.HouseholdId)},
                name AS {nameof(CompartmentResponse.Name)},
                notes AS {nameof(CompartmentResponse.Notes)}
            FROM compartments
            WHERE id = @CompartmentId
            """;
        CommandDefinition command = new(sql, new { query.CompartmentId }, cancellationToken: cancellationToken);
        CompartmentResponse? compartment = await connection.QuerySingleOrDefaultAsync<CompartmentResponse>(command);
        if (compartment is null)
        {
            return Result.Failure<CompartmentWithFoodItemPreviewResponse>(CompartmentErrors.NotFound);
        }
        IPagedList<FoodItemPreview> previews =
                await session.Query<FoodItemPreview>()
                    .Where(p => p.CompartmentId == query.CompartmentId && p.Quantity > 0)
                    .Where(p => string.IsNullOrEmpty(query.SearchText) || p.WebStyleSearch(query.SearchText))
                    .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        var response = new CompartmentWithFoodItemPreviewResponse(
            compartment.Id, compartment.StorageId, compartment.HouseholdId, compartment.Name, compartment.Notes, previews);
        
        return response;
    }
}
