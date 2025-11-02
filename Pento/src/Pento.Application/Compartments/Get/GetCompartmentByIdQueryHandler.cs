using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;

namespace Pento.Application.Compartments.Get;

internal sealed class GetCompartmentByIdQueryHandler(ISqlConnectionFactory connectionFactory) : IQueryHandler<GetCompartmentByIdQuery, CompartmentResponse>
{
    public async Task<Result<CompartmentResponse>> Handle(GetCompartmentByIdQuery request, CancellationToken cancellationToken)
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
            FROM storages
            WHERE id = @StorageId
            """;
        CommandDefinition command = new(sql, new { StorageId = request.CompartmentId }, cancellationToken: cancellationToken);
        CompartmentResponse? compartment = await connection.QuerySingleOrDefaultAsync<CompartmentResponse>(command);
        if (compartment is null)
        {
            return Result.Failure<CompartmentResponse>(CompartmentErrors.NotFound);
        }
        return compartment;
    }
}
