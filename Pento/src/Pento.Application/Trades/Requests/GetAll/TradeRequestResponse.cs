using System.Data.Common;
using Dapper;
using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.GetAll;

public sealed record TradeRequestResponse
{
    public Guid TradeRequestId {  get; init; }
    public Guid TradeOfferId { get; init; }
    public string OfferHouseholdName { get; init; }
    public string RequestHouseholdName { get; init; }
    public string Status { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public int TotalItems { get; init; }
}
public sealed record TradeOfferResponse
{
    public Guid TradeOfferId { get; init; }
    public string OfferHouseholdName { get; init; }
    public string Status { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public int TotalItems { get; init; }
}
public enum GetTradeRequestsSortBy
{
    CreatedOn,
    TotalItems
}
public sealed record GetTradeRequestsQuery(
    Guid? OfferId,
    TradeRequestStatus? Status, 
    bool? IsMine,
    GetTradeRequestsSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber, 
    int PageSize) : IQuery<PagedList<TradeRequestResponse>>;
internal sealed class GetTradeRequestsQueryValidator : AbstractValidator<GetTradeRequestsQuery>
{
    public GetTradeRequestsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.SortBy)        
            .IsInEnum().WithMessage("Invalid sort by option.")
            .When(x => x.SortBy.HasValue);
        RuleFor(x => x.SortOrder)
            .IsInEnum().WithMessage("Invalid sort order option.")
            .When(x => x.SortOrder.HasValue);
    }
}
internal sealed class GetTradeRequestsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory
    ) : IQueryHandler<GetTradeRequestsQuery, PagedList<TradeRequestResponse>>
{
    public async Task<Result<PagedList<TradeRequestResponse>>> Handle(
        GetTradeRequestsQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<PagedList<TradeRequestResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetTradeRequestsSortBy.CreatedOn => "tr.created_on",
            GetTradeRequestsSortBy.TotalItems => "8",
            _ => "tr.id"
        };
        string orderClause = $"ORDER BY {orderBy} {(query.SortOrder != null ? query.SortOrder.ToString() : "ASC")}";
        var parameters = new DynamicParameters();
        var filters = new List<string>();
        parameters.Add("@HouseholdId", householdId.Value);
        filters.Add("tr.household_id = @HouseholdId");
        if (query.OfferId.HasValue)
        {
            parameters.Add("@OfferId", query.OfferId.Value);
            filters.Add("tr.trade_offer_id = @OfferId");
        }
        if (query.Status.HasValue)
        {
            parameters.Add("@Status", query.Status.Value.ToString());
            filters.Add("tr.status = @Status");
        }
        if (query.IsMine.HasValue)
        {
            if (query.IsMine.Value)
            {
                parameters.Add("@UserId", userContext.UserId);
                filters.Add("tr.user_id = @UserId");
            }
            else
            {
                parameters.Add("@UserId", userContext.UserId);
                filters.Add("tr.user_id <> @UserId");
            }
            
        }
        
        parameters.Add("@Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("@PageSize", query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string sql = $@"
            SELECT 
                tr.id AS TradeRequestId,
                tr.trade_offer_id AS TradeOfferId,
                h1.name AS OfferHouseholdName,
                h2.name AS RequestHouseholdName,
                tr.status AS Status,
                tr.created_on AS CreatedOn,
                tr.updated_on AS UpdatedOn,
                COALESCE(
                    (SELECT COUNT(*) FROM trade_items ti
                     WHERE ti.request_id = tr.id), 0) AS TotalItems
            FROM trade_requests tr
            JOIN trade_offers tof ON tr.trade_offer_id = tof.id
            JOIN households h1 ON tof.household_id = h1.id
            JOIN households h2 ON tr.household_id = h2.id
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*) FROM trade_requests tr
            {whereClause};
        ";
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        IEnumerable<TradeRequestResponse> items = await multi.ReadAsync<TradeRequestResponse>();
        int totalCount = await multi.ReadFirstAsync<int>();
        var pagedList = PagedList<TradeRequestResponse>.Create(
            items,
            count: totalCount,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize);
        return pagedList;
    }
}
