using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Payments.GetById;
using Pento.Domain.Abstractions;

namespace Pento.Application.Payments.GetAll;

internal sealed class GetPaymentsQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetPaymentsQuery, PagedList<PaymentPreview>>
{
    public async Task<Result<PagedList<PaymentPreview>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        var filters = new List<string>
        {
            "is_deleted IS FALSE",
            "user_id = @UserId"
        };
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userContext.UserId);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            filters.Add("description ILIKE @SearchText");
            parameters.Add("SearchText", $"%{request.SearchText}%");
        }
        if (request.FromAmount.HasValue)
        {
            filters.Add("amount_due >= @FromAmount");
            parameters.Add("FromAmount", request.FromAmount.Value);
        }
        if (request.ToAmount.HasValue)
        {
            filters.Add("amount_due <= @ToAmount");
            parameters.Add("ToAmount", request.ToAmount.Value);
        }
        if (request.FromDate.HasValue)
        {
            filters.Add("created_at >= @FromDate");
            parameters.Add("FromDate", request.FromDate.Value);
        }
        if (request.ToDate.HasValue)
        {
            filters.Add("created_at <= @ToDate");
            parameters.Add("ToDate", request.ToDate.Value);
        }
        if (request.Status.HasValue)
        {
            filters.Add("status = @Status");
            parameters.Add("Status", request.Status.Value.ToString());
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        
        string sql = 
            $"""
            SELECT COUNT(*) FROM payments {whereClause};
            SELECT 
                id AS PaymentId,
                order_code AS OrderCode,
                description AS Description,
                CONCAT(amount_due::text, ' ', currency) AS Amount,
                status AS Status,
                created_at AS CreatedAt
            FROM payments
            {whereClause}
            ORDER BY created_at DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY;          
         """;
        parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<PaymentPreview> items = await multi.ReadAsync<PaymentPreview>();
        var pagedList = PagedList<PaymentPreview>.Create(items, totalCount, request.PageNumber, request.PageSize);
        return pagedList;
    }
}
