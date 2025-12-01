using Pento.Application.Abstractions.Messaging;
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
    bool? IsDeleted,
    int PageNumber,
    int PageSize) : IQuery<AdminPaymentsResponse>;
public enum Period
{
    Daily, Weekly, Monthly, Quarterly, Yearly, All
}
