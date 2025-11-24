using FluentValidation;

namespace Pento.Application.Payments.Cancel;

internal sealed class CancelPaymentCommandValidator : AbstractValidator<CancelPaymentCommand>
{
    public CancelPaymentCommandValidator()
    {
        RuleFor(c => c.PaymentId)
            .NotEqual(Guid.Empty).WithMessage("Payment Id must not be empty.");
        RuleFor(c => c.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}
