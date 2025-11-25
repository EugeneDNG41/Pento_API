using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Payments;

namespace Pento.Application.Payments.GetAll;

public sealed record GetPaymentsQuery(
    string? SearchText, 
    long? FromAmount, 
    long? ToAmount, 
    DateTime? FromDate,
    DateTime? ToDate,
    PaymentStatus? Status,
    int PageNumber,
    int PageSize) : IQuery<PagedList<PaymentPreview>>;
