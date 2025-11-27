using FluentValidation;

namespace Pento.Application.Payments.Delete;

internal sealed class DeletePaymentCommandValidator : AbstractValidator<DeletePaymentCommand>
{
    public DeletePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("Payment Id is required.");
    }
}
