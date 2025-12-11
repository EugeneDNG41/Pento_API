using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Payments.GetAll;

public sealed record AdminPaymentsResponse(
    AdminPaymentSummary Summary,
    PagedList<AdminPaymentPreview> Payments
    );
public sealed record AdminPaymentSummary
{
    public string TotalDue { get; init; }
    public string TotalPaid { get; init; }
    public int Pending { get; init; }
    public int Paid { get; init; }
    public int Failed { get; init; }
    public int Cancelled { get; init; }
    public int Expired { get; init; }

}
public sealed record AdminPaymentPreview
{
    public Guid PaymentId { get; init; }
    public Guid UserId { get; init; }
    public long OrderCode { get; init; }
    public string Description { get; init; }
    public string AmountDue { get; init; }
    public string AmountPaid { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
}


