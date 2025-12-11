using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Activities.GetSummary;

internal sealed class GetActivitySummaryQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetActivitySummaryQuery, IReadOnlyList<ActivitySummary>>
{
    public async Task<Result<IReadOnlyList<ActivitySummary>>> Handle(GetActivitySummaryQuery query, CancellationToken cancellationToken)
    {
        string dateTrunc = query.TimeWindow switch
        {
            TimeWindow.Weekly => "week",
            TimeWindow.Monthly => "month",
            TimeWindow.Quarterly => "quarter",
            TimeWindow.Yearly => "year",
            _ => "day"
        };
        string dateInterval = query.TimeWindow switch
        {
            TimeWindow.Weekly => "1 week",
            TimeWindow.Monthly => "1 month",
            TimeWindow.Quarterly => "3 months",
            TimeWindow.Yearly => "1 year",
            _ => "1 day"
        };
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string sql = $"""
            SELECT
              activity_code AS Code,
              name AS Name,
              CAST(ua.performed_on AS DATE) AS Date,
              COUNT(*) AS Count
              FROM user_activities ua
                  JOIN activities a ON ua.activity_code = a.code
              WHERE (@codes::text[] IS NULL OR ua.activity_code = ANY(@codes::text[]))
                    AND (@userIds::uuid[] IS NULL OR ua.user_id = ANY(@userIds::uuid[]))
                    AND (@householdIds::uuid[] IS NULL OR ua.household_id = ANY(@householdIds::uuid[]))
                    AND (@FromDate::timestamptz IS NULL OR ua.performed_on >= @FromDate::timestamptz)
                    AND (@ToDate::timestamptz   IS NULL OR ua.performed_on <= @ToDate::timestamptz)
              GROUP BY ua.activity_code, a.name, CAST(ua.performed_on AS DATE)
              ORDER BY CAST(ua.performed_on AS DATE);
        """;
        var parameters = new
        {
            dateTrunc,
            dateInterval,
            codes = string.IsNullOrEmpty(query.Codes?.FirstOrDefault()) ? null : query.Codes,
            userIds = query.UserIds?.FirstOrDefault() == Guid.Empty ? null : query.UserIds,
            householdIds = query.HouseholdIds?.FirstOrDefault() == Guid.Empty ? null : query.HouseholdIds,
            query.FromDate,
            query.ToDate
        };
        var command = new CommandDefinition(
            commandText: sql,
            parameters: parameters,
            cancellationToken: cancellationToken
        );
        var lookup = new Dictionary<string, ActivitySummary>();
        await connection.QueryAsync<ActivitySummary, ActivityCountByDate, ActivitySummary>(
            command: command,
            (summary, count) =>
            {
                if (lookup.TryGetValue(summary.Code, out ActivitySummary existingSub))
                {
                    summary = existingSub;
                }
                else
                {
                    lookup.Add(summary.Code, summary);
                }
                summary.CountByDate.Add(count);
                return summary;
            },
            splitOn: "Date"
        );
        return lookup.Values.ToList();
    }
}
