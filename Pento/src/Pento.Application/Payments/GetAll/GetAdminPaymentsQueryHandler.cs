using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Payments.GetAll;

internal sealed class GetAdminPaymentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetAdminPaymentsQuery, AdminPaymentsResponse>
{
    public async Task<Result<AdminPaymentsResponse>> Handle(GetAdminPaymentsQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetAdminPaymentsSortBy.OrderCode => "order_code",
            GetAdminPaymentsSortBy.Description => "description",
            GetAdminPaymentsSortBy.AmountDue => "amount_due",
            GetAdminPaymentsSortBy.AmountPaid => "amount_paid",
            GetAdminPaymentsSortBy.CreatedAt => "created_at",
            _ => "id"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder}";
        var filters = new List<string>();
        var parameters = new DynamicParameters();
        if (query.UserId.HasValue) 
        {
            filters.Add("user_id = @UserId");
            parameters.Add("UserId", query.UserId);
        }
        if (query.IsDeleted.HasValue)
        {
            filters.Add("is_deleted = @IsDeleted");
            parameters.Add("IsDeleted", query.IsDeleted.Value);
        }
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
            SELECT
            CONCAT(COALESCE(SUM(amount_due), 0)::text, ' ', currency) AS TotalDue,
            CONCAT(COALESCE(SUM(amount_paid), 0)::text, ' ', currency) AS TotalPaid,
            COUNT(CASE WHEN status = 'Pending' THEN 1 END) AS Pending,
            COUNT(CASE WHEN status = 'Paid' THEN 1 END) AS Paid,
            COUNT(CASE WHEN status = 'Failed' THEN 1 END) AS Failed,
            COUNT(CASE WHEN status = 'Cancelled' THEN 1 END) AS Cancelled,
            COUNT(CASE WHEN status = 'Expired' THEN 1 END) AS Expired
            FROM payments
            {whereClause}
            GROUP BY currency;
            SELECT COUNT(*) FROM payments {whereClause};
            SELECT 
                id AS PaymentId,
                order_code AS OrderCode,
                description AS Description,
                CONCAT(amount_due::text, ' ', currency) AS AmountDue,
                CONCAT(amount_paid::text, ' ', currency) AS AmountPaid,
                status AS Status,
                created_at AS CreatedAt,
                is_deleted AS IsDeleted
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
        AdminPaymentSummary? summary = await multi.ReadFirstOrDefaultAsync<AdminPaymentSummary>();
        summary ??= new AdminPaymentSummary
            {
                TotalDue = "0",
                TotalPaid = "0",
                Pending = 0,
                Paid = 0,
                Failed = 0,
                Cancelled = 0,
                Expired = 0
            };
        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<AdminPaymentPreview> items = await multi.ReadAsync<AdminPaymentPreview>();
        var pagedList = PagedList<AdminPaymentPreview>.Create(items, totalCount, query.PageNumber, query.PageSize);
        return new AdminPaymentsResponse(summary, pagedList);
    }
}
