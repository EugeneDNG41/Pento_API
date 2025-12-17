using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;

namespace Pento.Application.Notifications.Get;

internal sealed class GetNotificationsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<GetNotificationsQuery, PagedList<NotificationResponse>>
{
    public async Task<Result<PagedList<NotificationResponse>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        using DbConnection connection =
            await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        var filters = new List<string>();

        parameters.Add("@UserId", userId);
        filters.Add("n.user_id = @UserId");

        if (request.Type.HasValue)
        {
            parameters.Add("@Type", request.Type.Value.ToString());
            filters.Add("n.type = @Type");
        }

        string whereClause =
            "WHERE " + string.Join(" AND ", filters);

        string orderByClause =
            request.SortOrder == SortOrder.DESC
                ? "ORDER BY n.sent_on DESC"
                : "ORDER BY n.sent_on ASC";

        int offset = (request.PageNumber - 1) * request.PageSize;
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@Offset", offset);

        string sql = $"""
            SELECT COUNT(*)
            FROM notifications n
            {whereClause};

            SELECT
                n.id        AS Id,
                n.title     AS Title,
                n.body      AS Body,
                n.type      AS Type,
                n.data_json AS DataJson,
                n.sent_on   AS SentOn,
                n.read_on   AS ReadOn
            FROM notifications n
            {whereClause}
            {orderByClause}
            LIMIT @PageSize OFFSET @Offset;
            """;

        using SqlMapper.GridReader multi =
            await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        int totalCount = await multi.ReadFirstAsync<int>();
        var items =
            (await multi.ReadAsync<NotificationResponse>()).ToList();

        return Result.Success(
            new PagedList<NotificationResponse>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize));
    }
}
