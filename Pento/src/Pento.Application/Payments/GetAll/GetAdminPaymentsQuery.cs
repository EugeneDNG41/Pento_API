using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Payments;

namespace Pento.Application.Payments.GetAll;

public sealed record GetAdminPaymentsQuery(
    Guid? UserId,
    string? SearchText,
    long? FromAmount,
    long? ToAmount,
    DateTime? FromDate,
    DateTime? ToDate,
    PaymentStatus? Status,
    GetAdminPaymentsSortBy? SortBy,
    SortOrder SortOrder,
    bool? IsDeleted,
    int PageNumber,
    int PageSize) : IQuery<AdminPaymentsResponse>;
public enum GetAdminPaymentsSortBy
{
    OrderCode,
    Description,
    AmountDue,
    AmountPaid,
    CreatedAt
}
