using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Users.Search;

public sealed record SearchUserQuery(string? SearchText, bool? IsDeleted, int PageNumber, int PageSize) : IQuery<PagedList<UserPreview>>;

internal sealed class SearchUserQueryValidator : AbstractValidator<SearchUserQuery>
{
    public SearchUserQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
    }
}

internal sealed class SearchUserQueryQueryHandler(ISqlConnectionFactory dbConnectionFactory)
    : IQueryHandler<SearchUserQuery, PagedList<UserPreview>>
{
    public async Task<Result<PagedList<UserPreview>>> Handle(
        SearchUserQuery query,
        CancellationToken cancellationToken)
    {

        using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        const string sql = $"""
            SELECT 
                u.id AS UserId,
                h.name AS HouseholdName,
                u.email AS Email,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.created_at AS CreatedAt,
                u.updated_at AS UpdatedAt,
                u.is_deleted AS IsDeleted
            FROM users u
            LEFT JOIN households h ON u.household_id = h.id
            WHERE (@SearchText IS NULL OR 
                   h.name ILIKE '%' || @SearchText || '%' OR
                   u.first_name ILIKE '%' || @SearchText || '%' OR 
                   u.last_name ILIKE '%' || @SearchText || '%' OR 
                   u.email ILIKE '%' || @SearchText || '%') AND
                   @IsDeleted IS NULL OR u.is_deleted = @IsDeleted
            ORDER BY UserId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
        const string countSql = $"""
            SELECT COUNT(*)
            FROM users u
            LEFT JOIN households h ON u.household_id = h.id
            WHERE (@SearchText IS NULL OR 
                   h.name ILIKE '%' || @SearchText || '%' OR
                   u.first_name ILIKE '%' || @SearchText || '%' OR 
                   u.last_name ILIKE '%' || @SearchText || '%' OR 
                   u.email ILIKE '%' || @SearchText || '%') AND
                   @IsDeleted IS NULL OR u.is_deleted = @IsDeleted;
            """;
        var parameters = new
        {
            query.IsDeleted,
            query.SearchText,
            Offset = (query.PageNumber - 1) * query.PageSize,
            query.PageSize
        };
        IEnumerable<UserPreview> users = await connection.QueryAsync<UserPreview>(sql, parameters);
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var pagedList = new PagedList<UserPreview>(
            users.ToList(),
            totalCount,
            query.PageNumber,
            query.PageSize);
        return pagedList;
    }
}
