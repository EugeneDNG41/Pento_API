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

public sealed record SearchUserQuery(string? SearchText, string? Roles, int PageNumber, int PageSize) : IQuery<PagedList<UserPreview>>;

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
        SearchUserQuery request,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        const string sql = $"""
            SELECT 
                u.id AS UserId,
                h.name AS HouseholdName,
                u.email AS Email
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.created_at AS CreatedAt,
                u.updated_at AS UpdatedAt,

            FROM users u
            LEFT JOIN user_roles ur ON u.id = ur.user_id
            LEFT JOIN
            WHERE (@SearchText IS NULL OR 
                   u.first_name ILIKE '%' || @SearchText || '%' OR 
                   u.last_name ILIKE '%' || @SearchText || '%' OR 
                   u.email ILIKE '%' || @SearchText || '%')
            ORDER BY u.last_name, u.first_name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
        const string countSql = $"""
            SELECT COUNT(*)
            FROM users u
            WHERE (@SearchText IS NULL OR 
                   u.first_name ILIKE '%' || @SearchText || '%' OR 
                   u.last_name ILIKE '%' || @SearchText || '%' OR 
                   u.email ILIKE '%' || @SearchText || '%');
            """;
        var parameters = new
        {
            request.SearchText,
            Offset = (request.PageNumber - 1) * request.PageSize,
            request.PageSize
        };
        IEnumerable<UserPreview> users = await connection.QueryAsync<UserPreview>(sql, parameters);
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var pagedList = new PagedList<UserPreview>(
            users.ToList(),
            totalCount,
            request.PageNumber,
            request.PageSize);
        return pagedList;
    }
}
