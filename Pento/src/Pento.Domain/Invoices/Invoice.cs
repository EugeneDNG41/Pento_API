using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Invoices;

public class Invoice : Entity
{
    public Guid UserId { get; private set; }
    public Guid OwnedSubscriptionId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "VND";
    public PaymentMethod Method { get; private set; }       
    public PaymentProvider Provider { get; private set; }
    public string ProviderTransactionId { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string? Description { get; private set; }
    public DateTime? ProcesseddAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
}





