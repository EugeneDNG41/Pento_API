using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.DietaryTags;

namespace Pento.Application.DietaryTags.Get;

internal sealed class GetDietaryTagByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetDietaryTagByIdQuery, DietaryTagResponse>
{
    public async Task<Result<DietaryTagResponse>> Handle(
        GetDietaryTagByIdQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
                id AS {nameof(DietaryTagResponse.Id)},
                name AS {nameof(DietaryTagResponse.Name)},
                description AS {nameof(DietaryTagResponse.Description)}
            FROM dietary_tags
            WHERE id = @DietaryTagId
            """;

        DietaryTagResponse? tag = await connection.QueryFirstOrDefaultAsync<DietaryTagResponse>(
            sql,
            new { DietaryTagId = request.Id });

        if (tag is null)
        {
            return Result.Failure<DietaryTagResponse>(DietaryTagErrors.NotFound);
        }

        return Result.Success(tag);
    }
}
