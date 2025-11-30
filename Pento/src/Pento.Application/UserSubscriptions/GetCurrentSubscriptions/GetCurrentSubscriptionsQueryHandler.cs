

using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.UserSubscriptions.GetCurrentSubscriptions;

internal sealed class GetCurrentSubscriptionsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetCurrentSubscriptionsQuery, IReadOnlyList<UserSubscriptionResponse>>
{
    public async Task<Result<IReadOnlyList<UserSubscriptionResponse>>> Handle(GetCurrentSubscriptionsQuery query, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            @"
            SELECT
                us.id AS UserSubscriptionId,
                s.id AS SubscriptionId,
                s.name AS SubscriptionName,
                us.status AS Status,
                us.start_date AS StartDate,
                us.end_date AS EndDate,
                us.paused_date AS PausedDate,
                us.cancelled_date AS CancelledDate,
                CASE
                    WHEN us.paused_date IS NOT NULL AND us.remaining_days_after_pause IS NOT NULL
                    THEN CONCAT(us.remaining_days_after_pause::text, ' ', 'day',
                        CASE 
                            WHEN COALESCE(us.remaining_days_after_pause,0) = 1 THEN '' ELSE 's' 
                        END)
                    WHEN us.cancelled_date IS NOT NULL
                    THEN 'Cancelled'
                    WHEN us.end_date IS NOT NULL AND us.cancelled_date IS NULL
                    THEN CONCAT((us.end_date::date - current_date)::text, ' ', 'day',
                        CASE 
                            WHEN COALESCE(us.end_date::date - current_date) = 1 THEN '' ELSE 's' 
                        END)
                    ELSE 'Lifetime'
                END AS Duration
            FROM user_subscriptions us
            INNER JOIN subscriptions s ON s.id = us.subscription_id
            WHERE (@SearchText IS NULL OR s.name ILIKE '%' || @SearchText || '%')
                AND (@Status IS NULL OR us.status = @Status)
                AND ( @FromDuration IS NULL OR 
                        ( us.end_date IS NULL AND us.paused_date IS NULL AND us.cancelled_date IS NULL) OR
                        ( us.end_date IS NOT NULL AND (us.end_date::date - current_date) >= @FromDuration ) OR
                        ( us.remaining_days_after_pause IS NOT NULL AND remaining_days_after_pause >= @FromDuration))
                AND ( @ToDuration IS NULL OR 
                        ( us.end_date IS NOT NULL AND (us.end_date::date- current_date) <= @ToDuration ) OR
                        ( us.remaining_days_after_pause IS NOT NULL AND remaining_days_after_pause <= @ToDuration))
                AND us.user_id = @UserId
                AND us.is_deleted IS FALSE
            ";
        CommandDefinition command = new(sql, new { 
            query.SearchText, 
            Status = query.Status.HasValue ? query.Status.ToString() : null , 
            query.FromDuration,
            query.ToDuration,
            userContext.UserId}, cancellationToken: cancellationToken);
        IEnumerable<UserSubscriptionResponse> userSubscriptions = await connection.QueryAsync<UserSubscriptionResponse>(command);
        return userSubscriptions.AsList();
    }
}
