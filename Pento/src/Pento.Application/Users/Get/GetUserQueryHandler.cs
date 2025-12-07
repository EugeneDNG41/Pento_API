using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.Get;

internal sealed class GetUserQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
             SELECT
                 u.id AS {nameof(UserResponseRow.Id)},
                 u.household_id AS {nameof(UserResponseRow.HouseholdId)},
                 h.name AS {nameof(UserResponseRow.HouseholdName)},
                 u.avatar_url AS {nameof(UserResponseRow.AvatarUrl)},
                 u.email AS {nameof(UserResponseRow.Email)},
                 u.first_name AS {nameof(UserResponseRow.FirstName)},
                 u.last_name AS {nameof(UserResponseRow.LastName)},
                 u.created_at AS {nameof(UserResponseRow.CreatedAt)},
                 string_agg(ur.role_name, ',') AS {nameof(UserResponseRow.Roles)}
             FROM users u
             LEFT JOIN user_roles ur ON ur.user_id = u.id
             LEFT JOIN households h ON h.id = u.household_id
             WHERE u.id = @UserId
             GROUP BY u.id, h.name;
             SELECT
                us.id AS {nameof(UserSubscriptionPreview.UserSubscriptionId)},
                s.name AS {nameof(UserSubscriptionPreview.SubscriptionName)},
                CASE
                    WHEN us.end_date IS NOT NULL 
                    THEN CONCAT((us.end_date::date - current_date)::text, ' ', 'day',
                        CASE 
                            WHEN COALESCE(us.end_date::date - current_date,0) = 1 THEN '' ELSE 's' 
                        END)
                    ELSE 'Lifetime'
                END AS {nameof(UserSubscriptionPreview.Duration)}
                FROM user_subscriptions us
                INNER JOIN subscriptions s ON s.id = us.subscription_id
                WHERE us.user_id = @UserId AND (us.end_date IS NULL OR us.end_date > CURRENT_DATE) AND us.Status = @Status;
             """;
        CommandDefinition command = new(sql, new { userContext.UserId, Status = SubscriptionStatus.Active.ToString() }, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        UserResponseRow? userRow = await multi.ReadSingleOrDefaultAsync<UserResponseRow>();
        if (userRow is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }
        IEnumerable<UserSubscriptionPreview> activeSubcriptions = await multi.ReadAsync<UserSubscriptionPreview>();
        UserResponse user = new(
            Id: userRow.Id,
            HouseholdId: userRow.HouseholdId,
            HouseholdName: userRow.HouseholdName,
            AvatarUrl: userRow.AvatarUrl,
            Email: userRow.Email,
            FirstName: userRow.FirstName,
            LastName: userRow.LastName,
            CreatedAt: userRow.CreatedAt,
            Roles: userRow.Roles,
            ActiveSubscriptions: activeSubcriptions.ToList()
        );
        return user;
    }
}
