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
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
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
            WHERE id = @CompartmentId and is_deleted = false
            """;
        CommandDefinition command = new(sql, new { query.CompartmentId }, cancellationToken: cancellationToken);
        CompartmentResponse? compartment = await connection.QuerySingleOrDefaultAsync<CompartmentResponse>(command);
        if (compartment is null)
        {
            return Result.Failure<CompartmentWithFoodItemPreviewResponse>(CompartmentErrors.NotFound);
        }
        IReadOnlyList<Guid> ids = await session.Query<FoodItem>()
            .Where(f => f.CompartmentId == query.CompartmentId && f.Quantity > 0)
            .Select(f => f.Id).ToListAsync(cancellationToken);
        IQueryable<FoodItemPreview> previewsQueryable =session.Query<FoodItemPreview>().Where(p => ids.Contains(p.Id));
        if (!string.IsNullOrEmpty(query.SearchText))
        {
            string trimmed = query.SearchText.Trim();
            previewsQueryable = previewsQueryable.Where(p => p.Name.Contains(trimmed) || p.FoodGroup.Contains(trimmed));
        }
        IPagedList<FoodItemPreview> previews = await previewsQueryable.ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        var response = new CompartmentWithFoodItemPreviewResponse(
            compartment.Id, compartment.StorageId, compartment.HouseholdId, compartment.Name, compartment.Notes, previews);
        
        return response;
    }
}
