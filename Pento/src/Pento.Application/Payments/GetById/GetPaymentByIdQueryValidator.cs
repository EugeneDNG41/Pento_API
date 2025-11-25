using FluentValidation;

namespace Pento.Application.Payments.GetById;

internal sealed class GetPaymentByIdQueryValidator : AbstractValidator<GetPaymentByIdQuery>
{
    public GetPaymentByIdQueryValidator()
    {
        RuleFor(q => q.PaymentId)
            .NotEmpty().WithMessage("Payment Id is required.");
    }
}
