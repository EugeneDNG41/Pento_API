using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Activities.GetSummary;

public sealed record GetActivitySummaryQuery(
    string[]? Codes,
    Guid[]? UserIds,
    Guid[]? HouseholdIds,
    DateOnly? FromDate,
    DateOnly? ToDate,
    TimeWindow? TimeWindow) : IQuery<IReadOnlyList<ActivitySummary>>;
public sealed record ActivitySummary
{
    public string Code { get; init; }
    public string Name { get; init; }
    public List<ActivityCountByDate> CountByDate { get; init; } = new();
}
public sealed record ActivityCountByDate
{
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public int Count { get; init; }
    
}
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
              COALESCE(@FromDate::date, date_trunc(@dateTrunc, ua.performed_on)::date) AS FromDate,
              COALESCE(@ToDate::date, (date_trunc(@dateTrunc, ua.performed_on) + (@dateInterval)::interval - interval '1 day')::date) AS ToDate,
              COUNT(*) AS Count
              FROM user_activities ua
                  JOIN activities a ON ua.activity_code = a.code
              WHERE (@codes::text[] IS NULL OR ua.activity_code = ANY(@codes::text[]))
                    AND (@userIds::uuid[] IS NULL OR ua.user_id = ANY(@userIds::uuid[]))
                    AND (@householdIds::uuid[] IS NULL OR ua.household_id = ANY(@householdIds::uuid[]))
                    AND (@FromDate IS NULL OR ua.performed_on >= @FromDate)
                    AND (@ToDate   IS NULL OR ua.performed_on <= @ToDate)
              GROUP BY ua.activity_code, a.name, date_trunc(@dateTrunc, ua.performed_on)
              ORDER BY COALESCE(@FromDate::date, date_trunc(@dateTrunc, ua.performed_on)::date), COALESCE(@ToDate::date, (date_trunc(@dateTrunc, ua.performed_on) + (@dateInterval)::interval - interval '1 day')::date);
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
            splitOn: "FromDate"
        );
        return lookup.Values.ToList();
    }
}
