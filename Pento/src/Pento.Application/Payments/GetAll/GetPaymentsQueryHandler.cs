using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Payments.GetById;
using Pento.Domain.Abstractions;

namespace Pento.Application.Payments.GetAll;

internal sealed class GetPaymentsQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetPaymentsQuery, PagedList<PaymentPreview>>
{
    public async Task<Result<PagedList<PaymentPreview>>> Handle(GetPaymentsQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetPaymentsSortBy.OrderCode => "order_code",
            GetPaymentsSortBy.Description => "description",
            GetPaymentsSortBy.AmountDue => "amount_due",
            GetPaymentsSortBy.AmountPaid => "amount_paid",
            GetPaymentsSortBy.CreatedAt => "created_at",
            _ => "id"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder}";
        var filters = new List<string>
        {
            "is_deleted IS FALSE",
            "user_id = @UserId"
        };
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userContext.UserId);
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            filters.Add("description ILIKE @SearchText");
            parameters.Add("SearchText", $"%{query.SearchText}%");
        }
        if (query.FromAmount.HasValue)
        {
            filters.Add("amount_due >= @FromAmount");
            parameters.Add("FromAmount", query.FromAmount.Value);
        }
        if (query.ToAmount.HasValue)
        {
            filters.Add("amount_due <= @ToAmount");
            parameters.Add("ToAmount", query.ToAmount.Value);
        }
        if (query.FromDate.HasValue)
        {
            filters.Add("created_at >= @FromDate");
            parameters.Add("FromDate", query.FromDate.Value);
        }
        if (query.ToDate.HasValue)
        {
            filters.Add("created_at <= @ToDate");
            parameters.Add("ToDate", query.ToDate.Value);
        }
        if (query.Status.HasValue)
        {
            filters.Add("status = @Status");
            parameters.Add("Status", query.Status.Value.ToString());
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        
        string sql = 
            $"""
            SELECT COUNT(*) FROM payments {whereClause};
            SELECT 
                id AS PaymentId,
                order_code AS OrderCode,
                description AS Description,
                CONCAT(amount_due::text, ' ', currency) AS AmountDue,
                CONCAT(amount_paid::text, ' ', currency) AS AmountPaid,
                status AS Status,
                created_at AS CreatedAt
            FROM payments
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY;          
         """;
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<PaymentPreview> items = await multi.ReadAsync<PaymentPreview>();
        var pagedList = PagedList<PaymentPreview>.Create(items, totalCount, query.PageNumber, query.PageSize);
        return pagedList;
    }
}
