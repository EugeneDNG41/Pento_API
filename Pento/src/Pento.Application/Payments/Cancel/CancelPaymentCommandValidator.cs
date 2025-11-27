using FluentValidation;

namespace Pento.Application.Payments.Cancel;

internal sealed class CancelPaymentCommandValidator : AbstractValidator<CancelPaymentCommand>
{
    public CancelPaymentCommandValidator()
    {
        RuleFor(c => c.PaymentId)
            .NotEmpty().WithMessage("Payment Code is required.");
        RuleFor(c => c.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}
