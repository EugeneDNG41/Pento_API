using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.DietaryTags;

namespace Pento.Application.DietaryTags.Get;

internal sealed class GetDietaryTagsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetDietaryTagsQuery, IReadOnlyList<DietaryTagResponse>>
{
    public async Task<Result<IReadOnlyList<DietaryTagResponse>>> Handle(
        GetDietaryTagsQuery request,
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
            ORDER BY name ASC
            """;

        var tags = (await connection.QueryAsync<DietaryTagResponse>(sql)).ToList();

        if (tags.Count == 0)
        {
            return Result.Failure<IReadOnlyList<DietaryTagResponse>>(
                DietaryTagErrors.NotFound
            );
        }

        return tags;
    }
}
